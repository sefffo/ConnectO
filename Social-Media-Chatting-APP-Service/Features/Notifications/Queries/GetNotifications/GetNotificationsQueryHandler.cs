using MediatR;
using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.Dtos;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler(
        IUnitOfWork unitOfWork
    ) : IRequestHandler<GetNotificationsQuery, Result<NotificationFeedDto>>
    {
        public async Task<Result<NotificationFeedDto>> Handle(
            GetNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                return Error.BadRequest("Notification.InvalidUser", "Invalid user ID.");

            var repo = unitOfWork.GetRepository<Notification, Guid>();

            var query = repo.Query()
                .Include(n => n.Actor)
                .Where(n => n.RecipientId == userId);

            if (request.Before.HasValue)
                query = query.Where(n => n.CreatedAt < request.Before.Value);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(request.PageSize + 1)
                .ToListAsync(cancellationToken);

            var hasMore = items.Count > request.PageSize;
            if (hasMore) items.RemoveAt(items.Count - 1);

            var unreadCount = await repo.Query()
                .CountAsync(n => n.RecipientId == userId && !n.IsRead, cancellationToken);

            var dtos = items.Select(n => new NotificationDto(
                n.Id,
                n.ActorId,
                n.Actor.UserName!,
                n.Actor.AvatarUrl,
                n.Type,
                n.ReferenceId,
                n.IsRead,
                n.CreatedAt
            )).ToList();

            return Result<NotificationFeedDto>.Ok(new NotificationFeedDto(
                dtos,
                unreadCount,
                hasMore,
                hasMore ? items.Last().CreatedAt : null
            ));
        }
    }
}
