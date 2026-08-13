using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetUSerPosts;

public record GetUserPostsQuery(string AuthorId, string UserId, DateTime? Cursor, int Limit =20)
    : IRequest<Result<PostFeedDto>>;