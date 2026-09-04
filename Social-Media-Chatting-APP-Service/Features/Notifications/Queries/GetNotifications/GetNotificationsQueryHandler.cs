using MediatR;
using Microsoft.AspNetCore.Identity;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.Dtos;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler(
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager
    ) : IRequestHandler<GetNotificationsQuery, Result<NotificationFeedDto>>
    {
        public async Task<Result<NotificationFeedDto>> Handle(
            GetNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                return Error.BadRequest("Notification.InvalidUser", "Invalid user ID.");

            var repo = unitOfWork.GetRepository<Notification, Guid>();

            var allForUser = (await repo.FindAllAsync(n => n.RecipientId == userId)).ToList();

            var filtered = allForUser.AsEnumerable();

            if (request.Before.HasValue)
                filtered = filtered.Where(n => n.CreatedAt < request.Before.Value);

            var ordered = filtered
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var hasMore = ordered.Count > request.PageSize;
            var page = ordered.Take(request.PageSize).ToList();

            var unreadCount = allForUser.Count(n => !n.IsRead);

            // Resolve actor display info via UserManager
            var actorIds = page.Select(n => n.ActorId.ToString()).Distinct().ToList();
            var actors = new Dictionary<string, AppUser>();
            foreach (var actorId in actorIds)
            {
                var actor = await userManager.FindByIdAsync(actorId);
                if (actor is not null)
                    actors[actorId] = actor;
            }

            var dtos = page.Select(n =>
            {
                actors.TryGetValue(n.ActorId.ToString(), out var actor);
                return new NotificationDto(
                    n.Id,
                    n.ActorId,
                    actor?.UserName ?? "Unknown",
                    actor?.ProfilePicture,
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
