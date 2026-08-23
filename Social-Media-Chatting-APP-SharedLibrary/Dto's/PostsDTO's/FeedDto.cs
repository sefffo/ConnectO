namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

/// <summary>
/// Combined feed response.
/// Posts list contains both Friend and Discover posts tagged with FeedSource.
/// The client uses FeedSource to optionally render a "Suggested" badge on Discover posts.
///
/// Pagination:
///   NextFriendsCursor — pass as friendsCursor on the next request (null = no more friend posts)
///   NextDiscoverPage  — pass as discoverPage on the next request (null = no more discover posts)
///   HasMoreFriends    — whether more friend posts exist
///   HasMoreDiscover   — whether more discover posts exist
/// </summary>
public record FeedDto(
    List<PostDto>  Posts,
    DateTime?      NextFriendsCursor,
    int?           NextDiscoverPage,
    bool           HasMoreFriends,
    bool           HasMoreDiscover
);
