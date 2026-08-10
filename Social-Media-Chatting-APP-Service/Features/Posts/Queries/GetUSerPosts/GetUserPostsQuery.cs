using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

public record GetUserPostsQuery(string AuthorId, string UserId, DateTime? Cursor, int Limit)
    : IRequest<Result<PostFeedDto>>;