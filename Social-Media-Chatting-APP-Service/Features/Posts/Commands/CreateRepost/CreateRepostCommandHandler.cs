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

        var author = await userManager.FindByIdAsync(request.AuthorId);
        if (author is null)
            return Error.NotFound("User.NotFound", "Author not found");

        // 1. Fetch and validate target post first — before using it
        var ogPost = await postRepo.GetByIdAsync(request.Dto.OriginalPostId);
        if (ogPost is null || ogPost.IsDeleted)
            return Error.NotFound("OG-Post.NotFound", "Original Post not found");

        // 2. Resolve to root — one hop is always enough
        var resolvedId = ogPost.PostType == PostType.Original
            ? ogPost.Id
            : ogPost.OriginalPostId!.Value;

        // 3. Duplicate check — only blocks double repost, not double quote
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

        // 4. Quote must have actual content
        if (request.Dto.PostType == PostType.Quote &&
            string.IsNullOrWhiteSpace(request.Dto.QuoteContent))
            return Error.BadRequest("Quote.EmptyContent", "Quote content cannot be empty");

        // 5. Build post — Repost always points to root, Quote keeps direct target
        var post = new Post()
        {
            AuthorId = request.AuthorId,
            Author = author,
            PostType = request.Dto.PostType,
            OriginalPostId = request.Dto.PostType == PostType.Repost
                ? resolvedId
                : request.Dto.OriginalPostId,
            QuoteContent = request.Dto.QuoteContent,
            CreatedAt = DateTime.UtcNow,
        };

        await postRepo.AddAsync(post);
        await unitOfWork.SaveChangesAsync();

        var mappedPost = mapper.Map<PostDto>(post);
        return Result<PostDto>.Ok(mappedPost);
    }
}