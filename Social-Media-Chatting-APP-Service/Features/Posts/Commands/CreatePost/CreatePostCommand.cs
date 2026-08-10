using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Commands.CreatePost;

public record CreatePostCommand(string AuthorId,CreatePostDto Dto) : IRequest<Result<PostDto>>;