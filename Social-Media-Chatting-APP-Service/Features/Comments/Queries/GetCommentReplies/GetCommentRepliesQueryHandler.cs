using AutoMapper;
using MediatR;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Interfaces;
using Social_Media_Chatting_APP_Service.Specification.Comments;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryHandler(
    IUnitOfWork unitofwork,
    IMapper mapper
) : IRequestHandler<GetCommentRepliesQuery, Result<CommentFeedDto>>
{
    public async Task<Result<CommentFeedDto>> Handle(GetCommentRepliesQuery request,
        CancellationToken cancellationToken)
    {
        var commentRepo = unitofwork.GetRepository<Comment,Guid>();
        var spec = new CommentRepliesSpecification(request.CommentId, request.Cursor, request.Limit);
        
        // check el parent comment asln mawgood 
        var parentComment = await commentRepo.GetByIdAsync(request.CommentId);
        if (parentComment == null || parentComment.IsDeleted)
            return Error.NotFound("Parent-Comment.NotFound", "Comment not found");
        var replies = await commentRepo.FindAllAsync(spec);

        // el pagination b'a 
        
        var hasNextPage = replies.Count()>request.Limit;
        if (hasNextPage)
        {
            //yrg3 el limit
            replies =  replies.Take(request.Limit).ToList();
        }
        // lw 3ndha next page yb'a akher comment hwa el next Cursor lw la' yb'a DateTime(null)
        var nextCursor = hasNextPage ? replies.Last().CreatedAt : (DateTime?)null;
        
        var mappedComments = mapper.Map<List<CommentDto>>(replies);
        
        foreach (var (commentDto, comment) in mappedComments.Zip(replies))
        {
            commentDto.IsLikedByMe = comment.CommentLikes.Any(l => l.UserId == request.UserId.ToString());
            commentDto.LikeCount = comment.CommentLikes.Count;
            commentDto.RepliesCount = comment.Replies.Count(r => !r.IsDeleted);
        }
        
        return Result<CommentFeedDto>.Ok(new CommentFeedDto(mappedComments, nextCursor, hasNextPage));


    }
}