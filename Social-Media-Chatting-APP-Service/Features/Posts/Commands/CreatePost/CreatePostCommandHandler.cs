using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<AppUser> userManager
) : IRequestHandler<CreatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();
        
        
        var author = await userManager.FindByIdAsync(request.AuthorId);
        if (author is null)
            return Error.NotFound("User.NotFound", "Author not found");

        var post = new Post()
        {
            AuthorId = request.AuthorId,
            Author = author,
            Content = request.Dto.Content,
            QuoteContent = request.Dto.QuoteContent,
            PostType = request.Dto.PostType,
            CreatedAt = DateTime.UtcNow,
        };

        if (request.Dto.PostType == PostType.Repost || request.Dto.PostType == PostType.Quote)
        {
            var originalPost = await postRepo.FindAsync(p => p.Id == request.Dto.OriginalPostId);
            if (originalPost is null || originalPost.IsDeleted == true)
            {
                return Error.NotFound("Post.NotFound", "Original post not found");
            }
        }

        await postRepo.AddAsync(post);
        await unitOfWork.SaveChangesAsync();

        if (request.Dto.MediaAsset is not null && request.Dto.MediaAsset.Any())
        {
            var mediaAssetRepo = unitOfWork.GetRepository<MediaAsset, Guid>();
            foreach (var asset in request.Dto.MediaAsset)
            {
                var mediaAsset = await mediaAssetRepo.FindAsync(m => m.PublicId == asset.PublicId && !m.IsDeleted);
                if (mediaAsset is not null)
                {
                    mediaAsset.PostId = post.Id;
                }
            }
        }
        await unitOfWork.SaveChangesAsync();
        var mappedPost = mapper.Map<PostDto>(post);
        return Result<PostDto>.Ok(mappedPost);
    }
}