using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.SoftDeletePost;

public record SoftDeletePostCommand(
    Guid AuthorId,
    Guid PostId
    ) : IRequest<Result<string>>;