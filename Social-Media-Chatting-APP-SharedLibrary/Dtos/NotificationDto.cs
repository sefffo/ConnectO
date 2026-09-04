using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_SharedLibrary.Dtos
{
    public record NotificationDto(
        Guid Id,
        Guid ActorId,
        string ActorUsername,
        string? ActorProfilePicture,
        NotificationType Type,
        Guid? ReferenceId,
        bool IsRead,
        DateTime CreatedAt
    );

    public record NotificationFeedDto(
        IReadOnlyList<NotificationDto> Items,
        int UnreadCount,
        bool HasMore,
        DateTime? NextCursor
    );
}
