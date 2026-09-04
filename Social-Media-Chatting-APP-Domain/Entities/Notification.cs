using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_Domain.Entities
{
    public class Notification : BaseEntity<Guid>
    {
        public string RecipientId { get; set; }
        public AppUser Recipient { get; set; } = null!;

        public string ActorId { get; set; }
        public AppUser Actor { get; set; } = null!;

        public NotificationType Type { get; set; }

        /// <summary>
        /// Optional: the ID of the related entity (PostId, CommentId, ConversationId, FriendshipId)
        /// </summary>
        public Guid? ReferenceId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
