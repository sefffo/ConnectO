using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Service.Features.Notifications.Commands.MarkAsRead;
using Social_Media_Chatting_APP_Service.Features.Notifications.Queries.GetNotifications;
using Social_Media_Chatting_APP_SharedLibrary.Dtos;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(ISender sender) : ApiBaseController
{
    /// <summary>
    /// Get paginated notification feed. Pass 'before' cursor from previous response for next page.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<NotificationFeedDto>>> GetNotifications(
        [FromQuery] DateTime? before,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetNotificationsQuery(userId!, before, pageSize));
        return HandleResult(result);
    }

    /// <summary>
    /// Mark notifications as read. Pass specific IDs in body, or empty body to mark ALL unread as read.
    /// </summary>
    [HttpPut("read")]
    public async Task<ActionResult<Result<bool>>> MarkAsRead([FromBody] List<Guid>? notificationIds)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new MarkNotificationsReadCommand(userId!, notificationIds));
        return HandleResult(result);
    }
}
