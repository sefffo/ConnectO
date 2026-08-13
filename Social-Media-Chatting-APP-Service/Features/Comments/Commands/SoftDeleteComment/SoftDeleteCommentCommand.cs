using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Comments.Commands.SoftDeleteComment;

public record SoftDeleteCommentCommand(Guid AuthorId, Guid PostId, Guid CommentId) : IRequest<Result<string>>;