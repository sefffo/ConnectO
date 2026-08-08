using Social_Media_Chatting_APP_Domain.Entities.Enums;

namespace Social_Media_Chatting_APP_Domain.Entities;

public class Post : BaseEntity<Guid>
{
    
    //nav prop 
    public string AuthorId { get; set; }
    public AppUser Author { get; set; }
    
    public string? Content { get; set; } // can be normal string post or image with text or a Video (media yani)
    
    public PostType PostType { get; set; }
    
    //self-relation for the reposts 
    public Guid? OriginalPostId { set; get; }
    public Post? OriginalPost { set; get; }
    public string? QuoteContent { set; get; }
    
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; } // for post edits and deletes
    public bool IsDeleted { set; get; }
    public DateTime? DeletedAt { set; get; }
    //nav props if i have a media upload 
    //media assets => for a post content if image or video 
    public ICollection<MediaAsset> MediaAssets { set; get; } = [];
    // comments 
    public ICollection<Comment> Comments { set; get; } = [];
    //Likes 
    public ICollection<PostLike> PostLikes { set; get; } = [];
    //Self relation for the reposts 
    public ICollection<Post> Reposts { set; get; } = [];
    
}