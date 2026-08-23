using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

/// <summary>
/// Indicates where in the feed this post came from.
/// Friend  = posted by a direct friend or yourself.
/// Discover = posted by a friend-of-a-friend (suggested content).
/// The frontend can use this to render a subtle "Suggested" badge on Discover posts.
/// </summary>
public enum FeedSource
{
    Friend  = 0,
    Discover = 1
}

public class PostDto()
{
    public Guid Id { get; set; }

    public PostType PostType { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { set; get; }

    public AuthorDto Author { set; get; }

    public List<MediaAssetDto> MediaAssets { set; get; } = [];

    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int RepostCount { get; set; }
    public bool IsLikedByMe { get; set; }

    // only populated when PostType is Repost or Quote
    public PostDto? OriginalPost { get; set; }
    public string? QuoteContent { get; set; }

    /// <summary>
    /// Set by the feed handler — tells the client whether this post
    /// came from a direct friend or from a discover (friend-of-friend) query.
    /// Always null when PostDto is used outside the feed context (e.g. GetPostById).
    /// </summary>
    public FeedSource? FeedSource { get; set; }
}
