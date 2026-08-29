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
    public async Task<Result<CommentFeedDto>> Handle(GetCommentRepliesQuery request, CancellationToken cancellationToken)
    {
        var commentRepo = unitofwork.GetRepository<Comment, Guid>();

        // Existence check only — no mutation, skip tracker
        var parentComment = await commentRepo.FindNoTrackingAsync(c => c.Id == request.CommentId);
        if (parentComment is null || parentComment.IsDeleted)
            return Error.NotFound("Parent-Comment.NotFound", "Comment not found");

        // Pure read list — skip tracker
        var spec    = new CommentRepliesSpecification(request.CommentId, request.Cursor, request.Limit);
        var replies = await commentRepo.FindAllNoTrackingAsync(spec);

        var hasNextPage = replies.Count() > request.Limit;
        if (hasNextPage)
            replies = replies.Take(request.Limit).ToList();

        var nextCursor     = hasNextPage ? replies.Last().CreatedAt : (DateTime?)null;
        var mappedComments = mapper.Map<List<CommentDto>>(replies);

        foreach (var (commentDto, comment) in mappedComments.Zip(replies))
        {
            commentDto.IsLikedByMe  = comment.CommentLikes.Any(l => l.UserId == request.UserId.ToString());
            commentDto.LikeCount    = comment.CommentLikes.Count;
            commentDto.RepliesCount = comment.Replies.Count(r => !r.IsDeleted);
        }

        return Result<CommentFeedDto>.Ok(new CommentFeedDto(mappedComments, nextCursor, hasNextPage));
    }
}
