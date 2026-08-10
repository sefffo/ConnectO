using Social_Media_Chatting_APP_Domain.Entities.Enums;
using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

namespace Social_Media_Chatting_APP_SharedLibrary.Dto_s.PostsDTO_s;

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

    // only populated when PostType is Repost or Quote ==> 3shan el self-relation
    public PostDto? OriginalPost { get; set; }
    public string? QuoteContent { get; set; }
    
}