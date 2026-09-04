using MediatR;
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

            // Fetch page + 1 to determine hasMore
            var allForUser = (await repo.FindAllAsync(n => n.RecipientId == userId)).ToList();

            var filtered = allForUser.AsEnumerable();

            if (request.Before.HasValue)
                filtered = filtered.Where(n => n.CreatedAt < request.Before.Value);

            var ordered = filtered
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var hasMore = ordered.Count > request.PageSize;
            var page = ordered.Take(request.PageSize).ToList();

            // Unread count across ALL user notifications (not just this page)
            var unreadCount = allForUser.Count(n => !n.IsRead);

            // Load Actor info for each notification
            var actorIds = page.Select(n => n.ActorId).Distinct().ToList();
            var userRepo = unitOfWork.GetRepository<AppUser, string>();
            var actors = new Dictionary<Guid, AppUser>();
            foreach (var actorId in actorIds)
            {
                var actor = await repo.FindAsync(n => n.ActorId == actorId);
                if (actor is not null)
                    actors.TryAdd(actorId, actor.Actor);
            }

            var dtos = page.Select(n =>
            {
                actors.TryGetValue(n.ActorId, out var actor);
                return new NotificationDto(
                    n.Id,
                    n.ActorId,
                    actor?.UserName ?? "Unknown",
                    actor?.AvatarUrl,
                    n.Type,
                    n.ReferenceId,
                    n.IsRead,
                    n.CreatedAt
                );
            }).ToList();

            return Result<NotificationFeedDto>.Ok(new NotificationFeedDto(
                dtos,
                unreadCount,
                hasMore,
                hasMore ? page.Last().CreatedAt : null
            ));
        }
    }
}
