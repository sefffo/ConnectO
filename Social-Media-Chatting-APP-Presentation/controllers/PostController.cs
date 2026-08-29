using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreatePost;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreateRepost;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.SoftDeletePost;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetFeed;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetPostById;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetUSerPosts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(ISender sender) : ApiBaseController
{
    // ─── CREATE ───────────────────────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> Post([FromBody] CreatePostDto createPostDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new CreatePostCommand(user, createPostDto));
        return HandleResult(result);
    }

    // ─── DELETE ───────────────────────────────────────────────────────────────

    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> Post([FromRoute] Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new SoftDeletePostCommand(user, postId));
        return HandleResult(result);
    }

    // ─── GET SINGLE POST ──────────────────────────────────────────────────────

    [HttpGet("{postId}")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> GetPostById([FromRoute] Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetPostByIdQuery(postId, user));
        return HandleResult(result);
    }

    // ─── GET USER POSTS (by authorId) ─────────────────────────────────────────

    [Authorize]
    [HttpGet("posts/user/{authorId}")]
    public async Task<ActionResult<Result<PostFeedDto>>> GetUserPosts(
        [FromRoute] string authorId,
        [FromQuery] DateTime? cursor,
        [FromQuery] int limit = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetUserPostsQuery(authorId, userId, cursor, limit));
        return HandleResult(result);
    }

    // ─── GET MY OWN POSTS (profile shortcut) ──────────────────────────────────
    /// <summary>
    /// Shortcut for the profile screen — no need to know your own ID.
    /// Reads userId from the JWT and delegates to GetUserPostsQuery.
    /// </summary>
    [Authorize]
    [HttpGet("posts/mine")]
    public async Task<ActionResult<Result<PostFeedDto>>> GetMyPosts(
        [FromQuery] DateTime? cursor,
        [FromQuery] int limit = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(new GetUserPostsQuery(userId, userId, cursor, limit));
        return HandleResult(result);
    }

    // ─── HOME FEED ────────────────────────────────────────────────────────────
    /// <summary>
    /// Combined home feed:
    ///   - FeedSource.Friend  : posts from direct friends + yourself (newest first)
    ///   - FeedSource.Discover: posts from friends-of-friends (popularity sorted)
    ///
    /// Pagination:
    ///   friendsCursor  — DateTime from NextFriendsCursor of the previous response
    ///   discoverPage   — int from NextDiscoverPage of the previous response (default 0)
    /// </summary>
    [Authorize]
    [HttpGet("feed")]
    public async Task<ActionResult<Result<FeedDto>>> GetFeed(
        [FromQuery] DateTime? friendsCursor,
        [FromQuery] int discoverPage = 0,
        [FromQuery] int limit = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(
            new GetFeedQuery(userId, friendsCursor, discoverPage, limit));
        return HandleResult(result);
    }

    // ─── REPOST ───────────────────────────────────────────────────────────────
    [HttpPost("repost")]
    [Authorize]
    public async Task<ActionResult<Result<PostDto>>> CreateRepost([FromBody] CreateRepostDto createRepostDto)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new CreateRepostCommand(user, createRepostDto));
        return HandleResult(result);
    }
}