using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dtos;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Notifications.Queries.GetNotifications
{
    public record GetNotificationsQuery(
        string UserId,
        DateTime? Before,
        int PageSize = 20
    ) : IRequest<Result<NotificationFeedDto>>;
}
