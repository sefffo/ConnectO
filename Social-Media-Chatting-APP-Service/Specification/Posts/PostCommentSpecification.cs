using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Posts;

public class PostCommentSpecification : BaseSpecification<Comment>
{
    public PostCommentSpecification(Guid postId, DateTime? cursor, int size) : base(c =>
        c.PostId == postId && c.IsDeleted == false && (cursor == null || c.CreatedAt < cursor))
    {
        AddIncludes(c => c.Author);
        AddIncludes(c => c.CommentLikes);
        ApplyOrderByDescending(c=>c.CreatedAt);
        AddIncludes(c => c.Replies);
        ApplyTake(size);
        AddIncludes(c => c.MediaAssets);
    }
}