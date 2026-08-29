using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreatePost;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreateRepost;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.EditPost;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.SoftDeletePost;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetPostById;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetUSerPosts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(ISender sender) : ApiBaseController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> Post(CreatePostDto createPostDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new CreatePostCommand(user, createPostDto));
        return HandleResult(result);
    }

    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> DeletePost([FromRoute] Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new SoftDeletePostCommand(user, postId));
        return HandleResult(result);
    }

    /// <summary>
    /// Edit a post's text content only.
    /// Media assets attached to this post are permanently locked and cannot be changed.
    /// To remove media you must delete the entire post.
    /// </summary>
    [HttpPut("{postId}")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> EditPost(
        [FromRoute] Guid postId,
        [FromBody] EditPostDto editPostDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new EditPostCommand(user, postId, editPostDto));
        return HandleResult(result);
    }

    [HttpGet("{postId}")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> GetPostById([FromRoute] Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetPostByIdQuery(postId, user));
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("posts/user/{authorId}")]
    public async Task<ActionResult<Result<PostFeedDto>>> GetUserPosts([FromRoute] string authorId,
        [FromQuery] DateTime? cursor, [FromQuery] int limit = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetUserPostsQuery(authorId, userId, cursor, limit));
        return HandleResult(result);
    }

    [HttpPost("repost")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> CreateRepost([FromBody] CreateRepostDto createRepostDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new CreateRepostCommand(user, createRepostDto));
        return HandleResult(result);
    }
}
