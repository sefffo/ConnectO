using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Domain.Entities
{
    /// <summary>
    /// Stores a browser's Web Push subscription endpoint + encryption keys for a user.
    /// One user can have many subscriptions (phone browser, laptop browser, etc.).
    /// UserId is string because AppUser extends IdentityUser which uses string IDs.
    /// </summary>
    public class PushSubscription : BaseEntity<Guid>
    {
        /// <summary>Foreign key to AspNetUsers.Id (string, Identity convention)</summary>
        public string UserId { get; set; } = default!;

        /// <summary>The browser vendor push endpoint URL (unique per browser per device)</summary>
        public string Endpoint { get; set; } = default!;

        /// <summary>ECDH public key from the browser's PushSubscription.getKey('p256dh')</summary>
        public string P256dh { get; set; } = default!;

        /// <summary>Symmetric auth secret from the browser's PushSubscription.getKey('auth')</summary>
        public string Auth { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nav prop — back to AppUser
        public AppUser User { get; set; } = default!;
    }
}
