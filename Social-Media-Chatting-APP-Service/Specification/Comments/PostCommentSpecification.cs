using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Comments;

public class PostCommentSpecification : BaseSpecification<Comment>
{
    public PostCommentSpecification(Guid postId, DateTime? cursor, int size) : base(c =>
        c.PostId == postId && c.IsDeleted == false && (cursor == null || c.CreatedAt < cursor) && c.ParentCommentId == null)
    {
        AddIncludes(c => c.Author);
        AddIncludes(c => c.CommentLikes);
        ApplyOrderBy(c=>c.CreatedAt);
        AddIncludes(c => c.Replies);
        ApplyTake(size+1);
        AddIncludes(c => c.MediaAsset);
    }
}