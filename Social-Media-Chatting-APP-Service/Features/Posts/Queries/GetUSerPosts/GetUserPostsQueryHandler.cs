using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.Posts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetUSerPosts;

public class GetUserPostsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<GetUserPostsQuery, Result<PostFeedDto>>
{
    public async Task<Result<PostFeedDto>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
    {
        var postRepo = unitOfWork.GetRepository<Post, Guid>();

        // spec fetches size + 1 to detect next page
        var spec = new USerPostSpecifications(request.AuthorId, request.Cursor, request.Limit);
        var posts = await postRepo.FindAllAsync(spec);

        // determine if more pages exist, then trim the extra item
        var hasNextPage = posts.Count() > request.Limit;
        if (hasNextPage) posts = posts.Take(request.Limit).ToList();

        // cursor for next request = CreatedAt of last item in trimmed list
        var nextCursor = hasNextPage ? posts.Last().CreatedAt : (DateTime?)null;

        // map then manually set computed fields per post
        var dto = mapper.Map<List<PostDto>>(posts);
        foreach (var (postDto, post) in dto.Zip(posts))
        {
            postDto.IsLikedByMe = post.PostLikes.Any(l => l.UserId == request.UserId);
            postDto.LikeCount = post.PostLikes.Count;
            postDto.CommentCount = post.Comments.Count(c => !c.IsDeleted);
            postDto.RepostCount = post.Reposts.Count(r => !r.IsDeleted);
        }

        return Result<PostFeedDto>.Ok(new PostFeedDto(dto, nextCursor, hasNextPage));
    }
}