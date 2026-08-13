using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.Posts;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Queries.GetPostComments;

public class GetPostCommentsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<GetPostCommentsQuery,Result<CommentFeedDto>>
{
    public async Task<Result<CommentFeedDto>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var commentRepo = unitOfWork.GetRepository<Comment, Guid>();
        var spec = new PostCommentSpecification(request.PostId , request.Cursor , request.Limit);
        
        
        var postRepo = unitOfWork.GetRepository<Post, Guid>();
        
        var post = await postRepo.GetByIdAsync(request.PostId);

        if (post == null  || post.IsDeleted)
        {
            return Error.NotFound("Post.NotFound","Post Not Found");
        }
        
        
        
        var postComments = await commentRepo.FindAllAsync(spec);
        // bos ya3m saif 3shan tb'a fahem el pagination sah enta al awl btshoof lw el comments a'l f rag3 aktar mn size+1
        var hasNextPage = postComments.Count() > request.Limit;
        if (hasNextPage)
        {
            //yrg3 el limit
            postComments = postComments.Take(request.Limit).ToList();
        }
        var nextCursor = hasNextPage ? postComments.Last().CreatedAt : (DateTime?)null;
        var mappedComments = mapper.Map<List<CommentDto>>(postComments);
        
        foreach (var (commentDto, comment) in mappedComments.Zip(postComments))
        {
            commentDto.IsLikedByMe = comment.CommentLikes.Any(l => l.UserId == request.UserId.ToString());
            commentDto.LikeCount = comment.CommentLikes.Count;
            commentDto.RepliesCount = comment.Replies.Count(r => !r.IsDeleted);
        }
        
        
        
        return Result<CommentFeedDto>.Ok(new CommentFeedDto(mappedComments, nextCursor, hasNextPage));

        
        
        
    }
}