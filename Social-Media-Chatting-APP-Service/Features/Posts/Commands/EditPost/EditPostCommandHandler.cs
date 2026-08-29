using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.EditPost;

public class EditPostCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<EditPostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(EditPostCommand request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();

        var post = await postRepo.GetByIdAsync(request.PostId);

        if (post is null || post.IsDeleted == true)
            return Error.NotFound("Post.NotFound", "Post not found");

        if (post.AuthorId != request.UserId)
            return Error.Forbidden("Post.Forbidden", "You are not allowed to edit this post");

        // Facebook-style rule: content-only edit — media is permanently locked
        // If the caller tries to send media fields we simply ignore them (DTO has none).
        // Only text fields are updated.
        if (request.Dto.Content is not null)
            post.Content = request.Dto.Content;

        if (request.Dto.QuoteContent is not null)
            post.QuoteContent = request.Dto.QuoteContent;

        post.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        var mappedPost = mapper.Map<PostDto>(post);
        return Result<PostDto>.Ok(mappedPost);
    }
}
