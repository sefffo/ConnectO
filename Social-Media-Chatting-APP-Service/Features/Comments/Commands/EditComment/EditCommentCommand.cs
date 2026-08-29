using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.CommentDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.EditComment;

public record EditCommentCommand(
    string UserId,
    Guid CommentId,
    EditCommentDto Dto
) : IRequest<Result<CommentDto>>;
