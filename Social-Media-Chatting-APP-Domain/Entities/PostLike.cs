namespace Social_Media_Chatting_APP_Domain.Entities;

public class PostLike
{
    
    
    // junction table 
    
    // relation with the user first 
    
    
    public Guid PostId { get; set; }
    public Post Post { get; set; } //=> composite key 
    
    public string UserId { get; set; }
    public AppUser User { get; set; } //=> composite key 
    
    public DateTime LikedAt { set; get; }
    
    
    
    
    
}