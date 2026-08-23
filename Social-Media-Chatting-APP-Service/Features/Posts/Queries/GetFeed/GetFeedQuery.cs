using MediatR;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;
using Social_Media_Chatting_APP_SharedLibrary.SharedResponse;

namespace Social_Media_Chatting_APP_Service.Features.Posts.Queries.GetFeed;

/// <summary>
/// Returns a combined feed for the authenticated user:
///   - Tier 1 (FeedSource.Friend):   posts by direct friends + self, cursor-paginated newest-first
///   - Tier 2 (FeedSource.Discover): posts by friends-of-friends, popularity-sorted, injected into the same list
///
/// Pagination:
///   FriendsCursor  — DateTime cursor for the friends tier (pass nextFriendsCursor from last response)
///   DiscoverPage   — int offset for the discover tier (pass nextDiscoverPage from last response)
/// </summary>
public record GetFeedQuery(
    string  UserId,
    DateTime? FriendsCursor,
    int       DiscoverPage,
    int       Limit = 20
) : IRequest<Result<FeedDto>>;
