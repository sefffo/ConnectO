using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Notifications.Commands.MarkAsRead
{
    public class MarkNotificationsReadCommandHandler(
        IUnitOfWork unitOfWork
    ) : IRequestHandler<MarkNotificationsReadCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            MarkNotificationsReadCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                return Error.BadRequest("Notification.InvalidUser", "Invalid user ID.");

            var repo = unitOfWork.GetRepository<Notification, Guid>();

            IEnumerable<Notification> notifications;

            if (request.NotificationIds is { Count: > 0 })
            {
                notifications = await repo.FindAllAsync(
                    n => n.RecipientId == userId.ToString()
                         && !n.IsRead
                         && request.NotificationIds.Contains(n.Id));
            }
            else
            {
                notifications = await repo.FindAllAsync(
                    n => n.RecipientId == userId.ToString() && !n.IsRead);
            }

            foreach (var n in notifications)
            {
                n.IsRead = true;
                repo.Update(n);
            }

            await unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true);
        }
    }
}
