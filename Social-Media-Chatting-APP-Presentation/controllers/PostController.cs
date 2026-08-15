using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreatePost;
using Social_Media_Chatting_APP_Service.Features.Posts.Commands.SoftDeletePost;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetPostById;
using Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetUSerPosts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Presentation.Controllers;

[ApiController]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class PostController(ISender sender) : ApiBaseController
{
    [HttpPost]
    public async Task<ActionResult<Result<PostDto>>> Post(CreatePostDto createPostDto)
    {
        // find the user and check first 
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new CreatePostCommand(
            user,
            createPostDto
        ));
        return HandleResult(result);
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult<Result<PostDto>>> Post([FromQuery]Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new SoftDeletePostCommand(
            user,
            postId
        ));
        return HandleResult(result);
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<Result<PostDto>>> GetPostById([FromQuery] Guid postId)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetPostByIdQuery(postId , user));
        return HandleResult(result);
    }

    [HttpGet("posts/user/{authorId}")]
    public async Task<ActionResult<Result<PostFeedDto>>> GetUserPosts([FromRoute] string authorId , [FromQuery] DateTime? cursor , [FromQuery] int limit = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await sender.Send(new GetUserPostsQuery(authorId ,userId,cursor , limit ));
        return HandleResult(result);
    }
    
    
    
}