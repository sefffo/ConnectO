using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Posts;

/// <summary>
/// Fetches posts authored by the current user or any of their accepted friends.
/// Sorted newest-first with cursor-based pagination.
/// Fetches limit+1 so the handler can detect whether a next page exists.
/// </summary>
public class FeedPostSpecification : BaseSpecification<Post>
{
    public FeedPostSpecification(
        List<string> authorIds,   // current user id + all accepted friend ids
        DateTime? cursor,
        int size)
        : base(p =>
            authorIds.Contains(p.AuthorId) &&
            p.IsDeleted == false &&
            (cursor == null || p.CreatedAt < cursor))
    {
        AddIncludes(p => p.Author);
        AddIncludes(p => p.Comments);
        AddIncludes(p => p.Reposts);
        AddIncludes(p => p.MediaAssets);
        AddIncludes(p => p.PostLikes);
        AddThenIncludes(q => q.Include(p => p.OriginalPost!)
            .ThenInclude(op => op.Author));
        AddThenIncludes(q => q.Include(p => p.OriginalPost!)
            .ThenInclude(op => op.MediaAssets));

        ApplyOrderByDescending(p => p.CreatedAt);
        ApplyTake(size + 1);
    }
}
