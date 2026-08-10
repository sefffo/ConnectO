using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.SoftDeletePost;

public class SoftDeletePostCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<AppUser>userManager
) : IRequestHandler<SoftDeletePostCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SoftDeletePostCommand request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();

        var user = await userManager.FindByIdAsync(request.AuthorId.ToString());
        if (user is null)
        {
            return Error.Forbidden("User.Forbidden", "User not Allowed to delete this post");    
        }
        
        var post = await postRepo.GetByIdAsync(request.PostId);
        if (post is null)
        {
            return Error.NotFound("Post.NotFound", "Post not found");
        }

        post.IsDeleted = true;
        post.DeletedAt = DateTime.UtcNow;

        postRepo.Update(post);
        await unitOfWork.SaveChangesAsync();
        return Result<string>.Ok("Post.Deleted");
    }
}