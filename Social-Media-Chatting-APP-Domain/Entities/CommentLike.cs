namespace Social_Media_Chatting_APP_Domain.Entities;

public class CommentLike 
{
    
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } //=> composite key 
    
    public string UserId { get; set; }
    public AppUser User { get; set; } //=> composite key 
    
    public DateTime LikedAt { set; get; }

    
}