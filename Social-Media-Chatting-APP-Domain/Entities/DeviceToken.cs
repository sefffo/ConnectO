namespace Social_Media_Chatting_APP_Domain.Entities
{
    /// <summary>
    /// Stores a Firebase Cloud Messaging (FCM) device token for a user.
    /// One user can have many tokens (phone, tablet, multiple React Native installs).
    /// UserId is string because AppUser extends IdentityUser which uses string IDs.
    /// </summary>
    public class DeviceToken : BaseEntity<Guid>
    {
        /// <summary>Foreign key to AspNetUsers.Id (string, Identity convention)</summary>
        public string UserId { get; set; } = default!;

        /// <summary>
        /// The FCM registration token returned by Firebase SDK on the React Native side.
        /// Changes whenever the app reinstalls or the token rotates — always upsert, never insert blindly.
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// Optional device label to help with debugging (e.g. "iPhone 15", "Pixel 8").
        /// Sent by the client, not required.
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>Platform tag — "android" or "ios". Helps target platform-specific payloads in the future.</summary>
        public string Platform { get; set; } = "android";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Nav prop
        public AppUser User { get; set; } = default!;
    }
}
