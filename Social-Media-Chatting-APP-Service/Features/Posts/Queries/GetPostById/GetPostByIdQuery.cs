using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetPostById;

public record GetPostByIdQuery(
    Guid PostId,
    string AuthorId
) : IRequest<Result<PostDto>>;