using System.Net;
using System.Text.Json;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.PushSubscriptions;
using Social_Media_Chatting_APP_ServiceAbstraction;
using PushSubscription = Social_Media_Chatting_APP_Domain.Entities.PushSubscription;

namespace Social_Media_Chatting_APP_Service.Common;

/// <summary>
/// Concrete Web Push implementation using Lib.Net.Http.WebPush (VAPID).
/// Registered as Scoped — shares the same lifetime as the handler that calls it.
/// HttpClient is injected via IHttpClientFactory (registered in ServicesRegistration).
/// </summary>
public class PushNotificationService(
    IUnitOfWork unitOfWork,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<PushNotificationService> logger
) : IPushNotificationService
{
    public async Task SendAsync(string userId, string title, string body, string? url = null)
    {
        var repo = unitOfWork.GetRepository<PushSubscription, Guid>();
        var spec = new PushSubscriptionByUserIdSpecification(userId);
        var subscriptions = (await repo.FindAllAsync(spec)).ToList();

        if (subscriptions.Count == 0)
            return;

        var client = new PushServiceClient(httpClientFactory.CreateClient("WebPush"));
        client.DefaultAuthentication = new VapidAuthentication(
            configuration["WebPush:PublicKey"]!,
            configuration["WebPush:PrivateKey"]!)
        {
            Subject = configuration["WebPush:Subject"]!
        };

        // Trim body to 120 chars so the OS notification tray doesn't clip it awkwardly
        var trimmedBody = body.Length > 120 ? body[..120] + "\u2026" : body;

        var payload = JsonSerializer.Serialize(new { title, body = trimmedBody, url });
        var message = new PushMessage(payload);

        var deadSubscriptions = new List<PushSubscription>();

        foreach (var sub in subscriptions)
        {
            try
            {
                await client.RequestPushMessageDeliveryAsync(
                    new Lib.Net.Http.WebPush.PushSubscription
                    {
                        Endpoint = sub.Endpoint,
                        Keys = new Dictionary<string, string>
                        {
                            ["p256dh"] = sub.P256dh,
                            ["auth"] = sub.Auth
                        }
                    },
                    message);
            }
            catch (PushServiceClientException ex)
                when (ex.StatusCode is HttpStatusCode.Gone or HttpStatusCode.NotFound)
            {
                // Browser unsubscribed or the subscription expired — prune it
                deadSubscriptions.Add(sub);
                logger.LogInformation(
                    "Pruned expired push subscription {Endpoint} for user {UserId}",
                    sub.Endpoint, userId);
            }
            catch (Exception ex)
            {
                // Don't let a single failed delivery crash the whole send loop
                logger.LogWarning(ex,
                    "Push delivery failed for user {UserId} endpoint {Endpoint}",
                    userId, sub.Endpoint);
            }
        }

        // Batch-remove all dead subscriptions in one SaveChanges
        if (deadSubscriptions.Count > 0)
        {
            foreach (var dead in deadSubscriptions)
                repo.Remove(dead);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
