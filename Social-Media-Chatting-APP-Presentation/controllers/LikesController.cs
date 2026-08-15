using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Service.Features.Likes.Commands.LikeComment;
using Social_Media_Chatting_APP_Service.Features.Likes.Commands.LikePost;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikesController(ISender sender) : ApiBaseController
{
    [Authorize]
    [HttpPost("posts/{postId}")]
    public async Task<ActionResult<Result<bool>>> ToggleLikePost([FromRoute] Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new ToggleLikePostCommand(user, postId));
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost("comments/{commentId}")]
    public async Task<ActionResult<Result<bool>>> ToggleLikeComment([FromRoute] Guid commentId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new ToggleLikeCommentCommand(user, commentId));
        return HandleResult(result);
    }
}