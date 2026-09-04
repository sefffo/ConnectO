using MediatR;
using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Notifications.Commands.MarkAsRead
{
    public class MarkNotificationsReadCommandHandler(
        IUnitOfWork unitOfWork
    ) : IRequestHandler<MarkNotificationsReadCommand, Result>
    {
        public async Task<Result> Handle(
            MarkNotificationsReadCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                return Error.BadRequest("Notification.InvalidUser", "Invalid user ID.");

            var repo = unitOfWork.GetRepository<Notification, Guid>();

            var query = repo.Query()
                .Where(n => n.RecipientId == userId && !n.IsRead);

            if (request.NotificationIds is { Count: > 0 })
                query = query.Where(n => request.NotificationIds.Contains(n.Id));

            var notifications = await query.ToListAsync(cancellationToken);

            foreach (var n in notifications)
                n.IsRead = true;

            await unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
