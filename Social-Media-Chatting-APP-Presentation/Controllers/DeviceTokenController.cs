using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

/// <summary>
/// Manages FCM device token lifecycle for React Native mobile clients:
///   POST   /api/device-tokens/register    — stores or updates a token (idempotent)
///   DELETE /api/device-tokens/unregister  — removes a token on logout or permission revoke
/// </summary>
[Authorize]
[ApiController]
[Route("api/device-tokens")]
public class DeviceTokenController(
    IUnitOfWork unitOfWork
) : ControllerBase
{
    /// <summary>
    /// Registers an FCM token for the authenticated user's device.
    /// Idempotent: if the token already exists it updates the UserId + DeviceName
    /// (handles re-login on the same device scenario).
    /// The React Native app should call this on every app launch after Firebase
    /// resolves the token via messaging().getToken().
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDeviceTokenRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var repo = unitOfWork.GetRepository<DeviceToken, Guid>();

        var existing = await repo.FindAsync(d => d.Token == request.Token);

        if (existing is null)
        {
            await repo.AddAsync(new DeviceToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = request.Token,
                DeviceName = request.DeviceName,
                Platform = request.Platform ?? "android",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            // Token already registered — re-assign to current user (e.g. re-login)
            existing.UserId = userId;
            existing.DeviceName = request.DeviceName;
            existing.UpdatedAt = DateTime.UtcNow;
            repo.Update(existing);
        }

        await unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Unregisters an FCM token. Call this on logout or when the user
    /// revokes notification permissions in device settings.
    /// </summary>
    [HttpDelete("unregister")]
    public async Task<IActionResult> Unregister([FromBody] UnregisterDeviceTokenRequest request)
    {
        var repo = unitOfWork.GetRepository<DeviceToken, Guid>();
        var existing = await repo.FindAsync(d => d.Token == request.Token);

        if (existing is not null)
        {
            repo.Remove(existing);
            await unitOfWork.SaveChangesAsync();
        }

        return NoContent();
    }
}

// ---- Request DTOs ----
public record RegisterDeviceTokenRequest(string Token, string? DeviceName, string? Platform);
public record UnregisterDeviceTokenRequest(string Token);
