using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.DeviceTokens;
using Social_Media_Chatting_APP_ServiceAbstraction;

namespace Social_Media_Chatting_APP_Service.Common;

/// <summary>
/// Sends FCM notifications via the Firebase HTTP v1 API.
/// Uses a service account access token (OAuth2) — not the legacy server key.
///
/// Required appsettings keys:
///   Firebase:ProjectId       — your Firebase project ID
///   Firebase:ServiceAccountJson — full service account JSON (store in user-secrets / env var)
/// </summary>
public class FcmNotificationService(
    IUnitOfWork unitOfWork,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<FcmNotificationService> logger
) : IFcmNotificationService
{
    // FCM HTTP v1 endpoint template
    private const string FcmEndpoint =
        "https://fcm.googleapis.com/v1/projects/{0}/messages:send";

    public async Task SendAsync(
        string userId,
        string title,
        string body,
        Dictionary<string, string>? data = null)
    {
        var repo = unitOfWork.GetRepository<DeviceToken, Guid>();
        var spec = new DeviceTokenByUserIdSpecification(userId);
        var tokens = (await repo.FindAllAsync(spec)).ToList();

        if (tokens.Count == 0)
            return;

        var projectId = configuration["Firebase:ProjectId"]!;
        var accessToken = await GetAccessTokenAsync();
        var url = string.Format(FcmEndpoint, projectId);

        var client = httpClientFactory.CreateClient("Fcm");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var deadTokens = new List<DeviceToken>();

        foreach (var deviceToken in tokens)
        {
            var payload = new
            {
                message = new
                {
                    token = deviceToken.Token,
                    notification = new { title, body },
                    // Data payload — React Native app reads this on notification tap
                    data = data ?? new Dictionary<string, string>()
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, content);

                if (response.StatusCode is HttpStatusCode.NotFound)
                {
                    // Token is no longer valid — prune it
                    deadTokens.Add(deviceToken);
                    logger.LogInformation(
                        "Pruned stale FCM token for user {UserId} device {DeviceName}",
                        userId, deviceToken.DeviceName);
                }
                else if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    logger.LogWarning(
                        "FCM delivery failed for user {UserId}: {StatusCode} {Error}",
                        userId, response.StatusCode, err);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "FCM send exception for user {UserId} token {Token}",
                    userId, deviceToken.Token);
            }
        }

        if (deadTokens.Count > 0)
        {
            foreach (var dead in deadTokens)
                repo.Remove(dead);
            await unitOfWork.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Gets a short-lived OAuth2 access token using the Firebase service account JSON.
    /// Google.Apis.Auth is used for this — no manual JWT needed.
    /// </summary>
    private async Task<string> GetAccessTokenAsync()
    {
        var serviceAccountJson = configuration["Firebase:ServiceAccountJson"]!;

        var credential = Google.Apis.Auth.OAuth2.GoogleCredential
            .FromJson(serviceAccountJson)
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var token = await credential.UnderlyingCredential
            .GetAccessTokenForRequestAsync();

        return token;
    }
}
