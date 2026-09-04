namespace Social_Media_Chatting_APP_ServiceAbstraction;

/// <summary>
/// Abstraction over Firebase Cloud Messaging (FCM) HTTP v1 API.
/// Lives in ServiceAbstraction alongside IRealtimeNotifier and IPushNotificationService.
///
/// Used for React Native mobile apps — complements Web Push (browser) notifications.
/// Call this for offline users who have a registered FCM device token.
/// </summary>
public interface IFcmNotificationService
{
    /// <summary>
    /// Sends an FCM push notification to all registered devices for the user.
    /// Automatically prunes stale tokens (404/410 responses from Firebase).
    /// </summary>
    /// <param name="userId">AppUser.Id (string, Identity)</param>
    /// <param name="title">Notification title shown in the device tray</param>
    /// <param name="body">Notification body text</param>
    /// <param name="data">Optional key-value data payload for the React Native app to handle on tap</param>
    Task SendAsync(string userId, string title, string body, Dictionary<string, string>? data = null);
}
