using Social_Media_Chatting_APP_SharedLibrary.Dto_s.commonDTO_s;

public class CommentDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public MediaAssetDto? MediaAsset { get; set; }
    public Guid? ParentCommentId { get; set; }
    public AuthorDto Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int LikeCount { get; set; }
    public int RepliesCount { get; set; }
    public bool IsLikedByMe { get; set; }
}