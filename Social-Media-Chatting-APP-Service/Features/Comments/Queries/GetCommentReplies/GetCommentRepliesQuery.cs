using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Queries.GetCommentReplies;

public record GetCommentRepliesQuery(  
    Guid CommentId,
    Guid UserId // for is liked by ME 
    ,
    DateTime? Cursor,
    int Limit = 20) : IRequest<Result<CommentFeedDto>>;