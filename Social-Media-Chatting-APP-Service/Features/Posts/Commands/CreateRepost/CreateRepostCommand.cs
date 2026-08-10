using MediatR;
using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreateRepost;

public record CreateRepostCommand(
    string AuthorId,
    CreateRepostDto Dto
) : IRequest<Result<PostDto>>;