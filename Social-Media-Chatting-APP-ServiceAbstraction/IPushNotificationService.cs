namespace Social_Media_Chatting_APP_ServiceAbstraction;

/// <summary>
/// Abstraction over the Web Push Protocol (VAPID).
/// Lives in ServiceAbstraction alongside IRealtimeNotifier so that any handler
/// can depend on it without knowing about Lib.Net.Http.WebPush internals.
///
/// Trigger this ONLY when the user is offline (no active SignalR connection).
/// IRealtimeNotifier already handles the online path.
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to all registered browser subscriptions for the user.
    /// Automatically prunes dead (410 Gone) subscriptions.
    /// </summary>
    /// <param name="userId">AppUser.Id (string, Identity)</param>
    /// <param name="title">Notification title shown in the OS notification tray</param>
    /// <param name="body">Notification body text (trimmed to 120 chars internally)</param>
    /// <param name="url">Optional deep-link URL the browser opens on click</param>
    Task SendAsync(string userId, string title, string body, string? url = null);
}
