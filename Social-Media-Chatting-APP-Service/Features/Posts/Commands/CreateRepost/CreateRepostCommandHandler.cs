using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreateRepost;

public class CreateRepostCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<AppUser> userManager) : IRequestHandler<CreateRepostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(CreateRepostCommand request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();
        //check if the original Post found 
        var ogPost = await postRepo.GetByIdAsync(request.Dto.OriginalPostId);

        var resolvedId = ogPost.PostType == PostType.Original
            ? ogPost.Id
            : ogPost.OriginalPostId!.Value;
        if (ogPost is null || ogPost.IsDeleted)
        {
            return Error.NotFound("OG-Post.NotFound", "Original Post not found");
        }

        //    Duplicate repost check — only for Repost action, not Quote.
        //    A user can quote the same post multiple times but can only repost once.
        if (request.Dto.PostType == PostType.Repost)
        {
            var existingRepost = await postRepo.FindAsync(p =>
                p.AuthorId == request.AuthorId &&
                p.OriginalPostId == resolvedId &&
                p.PostType == PostType.Repost &&
                !p.IsDeleted);

            if (existingRepost is not null)
                return Error.BadRequest("Repost.Duplicate", "You already reposted this post");
        }

        if (request.Dto.PostType == PostType.Quote &&
            string.IsNullOrWhiteSpace(request.Dto.QuoteContent))
            return Error.BadRequest("Quote.EmptyContent", "Quote content cannot be ");
        
        var Post = new Post()
        {
            AuthorId = request.AuthorId, // teh one who reposted 
            PostType = request.Dto.PostType,
            OriginalPostId = request.Dto.PostType == PostType.Repost 
                ? resolvedId          // Repost → always root
                : request.Dto.OriginalPostId, // Quote → keep direct target
            QuoteContent = request.Dto.QuoteContent,
            CreatedAt = DateTime.UtcNow,
        };
        await postRepo.AddAsync(Post);
        await unitOfWork.SaveChangesAsync();
        var mappedPost = mapper.Map<PostDto>(Post);
        return Result<PostDto>.Ok(mappedPost);
    }
}