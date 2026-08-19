using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.Posts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<GetPostByIdQuery,Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();

        var spec = new PostDetailsSpecifications(request.PostId);
        var post = await postRepo.FindAsync(spec);
        if (post is null || post.IsDeleted)
        {
            return Error.NotFound("Post.NotFound","Post not found");
        }

        var dto = mapper.Map<PostDto>(post);

        // Resolve OriginalPost manually — AutoMapper can't safely handle
        // self-referential Post → PostDto mapping when OriginalPost is null.
        // This is consistent with how LikeCount, CommentCount etc. are handled below.
        if (post.OriginalPost is not null)
            dto.OriginalPost = mapper.Map<PostDto>(post.OriginalPost);

        dto.IsLikedByMe = post.PostLikes.Any(p => p.UserId == request.AuthorId);
        dto.LikeCount = post.PostLikes.Count;
        dto.CommentCount = post.Comments.Count(c => c.IsDeleted == false);
        dto.RepostCount = post.Reposts.Count(r => r.IsDeleted == false);

        return Result<PostDto>.Ok(dto);
    }
}
