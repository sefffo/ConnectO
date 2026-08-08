namespace Social_Media_Chatting_APP_Domain.Entities;

public class Comment : BaseEntity<Guid>
{
    //nav prop on which post contain that comment 
    public Guid PostId { get; set; }
    public Post Post { get; set; }
    // nav pro on who made that comment 
    public string AuthorId { get; set; }
    public AppUser Author { get; set; }
    
    public string? Content { get; set; } // as it can be an image or video
    
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; }
    public bool IsDeleted { set; get; }
    public DateTime? DeletedAt { set; get; }
    
    //media assets => for a post content if image or video 
    public ICollection<MediaAsset> MediaAssets { set; get; } = [];
    // for the replies to the comment
    // for the parent comment
    public Guid? ParentCommentId { set; get; }
    public Comment? ParentComment { set; get; }
    public ICollection<Comment> Replies { set; get; } = [];
    // for the likes on the comment 
    public ICollection<CommentLike> CommentLikes { set; get; } = [];
    
}