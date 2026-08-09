using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Posts;

public class CommentRepliesSpecification : BaseSpecification<Comment>
{
    public CommentRepliesSpecification(Guid parentCommentId , DateTime?cursor , int size) : base(
            c=>c.ParentCommentId == parentCommentId && c.IsDeleted == false && (cursor == null || c.CreatedAt < cursor)
        
        )
    {
        ApplyTake(size);
        AddIncludes(c=>c.Author);
        AddIncludes(c=>c.CommentLikes);
        ApplyOrderByDescending(c=>c.CreatedAt);
        AddIncludes(c=>c.Replies);
        AddIncludes(c=>c.MediaAssets);
    }
}