using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Service.Features.Comments.Commands.CreateComment;
using Social_Media_Chatting_APP_Service.Features.Comments.Commands.EditComment;
using Social_Media_Chatting_APP_Service.Features.Comments.Commands.SoftDeleteComment;
using Social_Media_Chatting_APP_Service.Features.Comments.Queries.GetCommentReplies;
using Social_Media_Chatting_APP_Service.Features.Comments.Queries.GetPostComments;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ISender sender) : ApiBaseController
{
    [Authorize]
    [HttpPost("{postId}/comments")]
    public async Task<ActionResult<Result<CommentDto>>> comment([FromBody] CreateCommentDto createCommentDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new CreateCommentCommand(user, createCommentDto));
        return HandleResult(result);
    }

    /// <summary>
    /// Edit a comment's text content only.
    /// The media asset attached to a comment is permanently locked.
    /// To remove the media you must delete the entire comment.
    /// </summary>
    [Authorize]
    [HttpPut("{postId}/comments/{commentId}")]
    public async Task<ActionResult<Result<CommentDto>>> EditComment(
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromBody] EditCommentDto editCommentDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new EditCommentCommand(user, commentId, editCommentDto));
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{postId}/comments/{commentId}")]
    public async Task<ActionResult<Result<CommentDto>>> softDeleteComment([FromRoute] Guid postId,
        [FromRoute] Guid commentId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new SoftDeleteCommentCommand(user, postId, commentId));
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{postId}/comments")]
    public async Task<ActionResult<Result<CommentFeedDto>>> GetPostComments([FromRoute] Guid postId,
        [FromQuery] DateTime? cursor,
        [FromQuery] int limit = 20)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetPostCommentsQuery(postId, user, cursor, limit));
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{commentId}/replies")]
    public async Task<ActionResult<Result<CommentFeedDto>>> GetCommentReplies([FromRoute] Guid commentId,
        [FromQuery] DateTime? cursor, [FromQuery] int limit = 15)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetCommentRepliesQuery(commentId, user, cursor, limit));
        return HandleResult(result);
    }
}
