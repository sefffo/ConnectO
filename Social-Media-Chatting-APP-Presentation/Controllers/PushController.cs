using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

/// <summary>
/// Manages browser push subscription lifecycle:
///   GET  /api/push/vapid-key   — returns the public VAPID key for the frontend
///   POST /api/push/subscribe    — stores a new browser subscription (idempotent)
///   DELETE /api/push/unsubscribe — removes a subscription when user opts out
/// </summary>
[Authorize]
[ApiController]
[Route("api/push")]
public class PushController(
    IUnitOfWork unitOfWork,
    IConfiguration configuration
) : ControllerBase
{
    /// <summary>
    /// Returns the VAPID public key so the frontend can pass it to
    /// pushManager.subscribe({ applicationServerKey: ... })
    /// </summary>
    [HttpGet("vapid-key")]
    [AllowAnonymous] // frontend calls this before the user logs in
    public IActionResult GetVapidKey()
        => Ok(new { publicKey = configuration["WebPush:PublicKey"] });

    /// <summary>
    /// Stores a browser's push subscription. Idempotent: if the endpoint already
    /// exists it updates the keys (browser can rotate keys after a period).
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var repo = unitOfWork.GetRepository<PushSubscription, Guid>();

        var existing = await repo.FindAsync(p => p.Endpoint == request.Endpoint);

        if (existing is null)
        {
            await repo.AddAsync(new PushSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Endpoint = request.Endpoint,
                P256dh = request.Keys.P256dh,
                Auth = request.Keys.Auth,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            // Endpoint already registered — update keys and re-assign to current user
            // (handles re-login on the same browser scenario)
            existing.UserId = userId;
            existing.P256dh = request.Keys.P256dh;
            existing.Auth = request.Keys.Auth;
            repo.Update(existing);
        }

        await unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Removes a push subscription. Called when user explicitly disables notifications
    /// or when the frontend detects the browser has unsubscribed.
    /// </summary>
    [HttpDelete("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
    {
        var repo = unitOfWork.GetRepository<PushSubscription, Guid>();
        var existing = await repo.FindAsync(p => p.Endpoint == request.Endpoint);

        if (existing is not null)
        {
            repo.Remove(existing);
            await unitOfWork.SaveChangesAsync();
        }

        return NoContent();
    }
}

// ---- Request DTOs (inline, no need for a separate file) ----

public record SubscribeRequest(string Endpoint, SubscriptionKeys Keys);
public record SubscriptionKeys(string P256dh, string Auth);
public record UnsubscribeRequest(string Endpoint);
