using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Notifications.Commands.MarkAsRead
{
    public record MarkNotificationsReadCommand(
        string UserId,
        /// <summary>If null or empty, marks ALL unread notifications as read.</summary>
        IReadOnlyList<Guid>? NotificationIds = null
    ) : IRequest<Result<bool>>;
}
