using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Likes.Commands.LikeComment;

public record ToggleLikePostCommand(
    string UserId,
    Guid PostId
) : IRequest<Result<bool>>;