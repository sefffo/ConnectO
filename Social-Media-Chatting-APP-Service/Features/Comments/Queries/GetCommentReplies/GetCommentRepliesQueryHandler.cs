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
        var spec = new CommentRepliesSpecification(request.CommentId, request.Cursor , request.Limit);
        
        // check el parent comment asln mawgood 
        var parentComment = await commentRepo.FindAllAsync(spec);
        if (parentComment == null)
        {
            return Error.NotFound("Parent-Comment.NotFound", "Comment not found");
        }

        // el pagination b'a 
        
        var hasNextPage = parentComment.Count()>request.Limit;
        if (hasNextPage)
        {
            //yrg3 el limit
            parentComment =  parentComment.Take(request.Limit).ToList();
        }
        // lw 3ndha next page yb'a akher comment hwa el next Cursor lw la' yb'a DateTime(null)
        var nextCursor = hasNextPage ? parentComment.Last().CreatedAt : (DateTime?)null;
        
        var mappedComments = mapper.Map<List<CommentDto>>(parentComment);
        
        foreach (var (commentDto, comment) in mappedComments.Zip(parentComment))
        {
            commentDto.IsLikedByMe = comment.CommentLikes.Any(l => l.UserId == request.UserId.ToString());
            commentDto.LikeCount = comment.CommentLikes.Count;
            commentDto.RepliesCount = comment.Replies.Count(r => !r.IsDeleted);
        }
        
        return Result<CommentFeedDto>.Ok(new CommentFeedDto(mappedComments, nextCursor, hasNextPage));


    }
}