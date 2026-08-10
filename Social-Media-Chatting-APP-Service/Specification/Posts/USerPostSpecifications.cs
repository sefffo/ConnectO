using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Posts;

public class USerPostSpecifications : BaseSpecification<Post>
{
    public USerPostSpecifications(string userId, DateTime? cursor, int size) : base(p =>
        p.IsDeleted == false && p.AuthorId == userId && (cursor == null || p.CreatedAt < cursor))
    {
        AddIncludes(p => p.Author);
        AddIncludes(p => p.Comments);
        AddIncludes(p => p.Reposts);
        AddIncludes(p=>p.MediaAssets);
        AddIncludes(p=>p.PostLikes);
        AddThenIncludes(q => q.Include(p => p.OriginalPost!)
            .ThenInclude(op => op.Author));

        AddThenIncludes(q => q.Include(p => p.OriginalPost!)
            .ThenInclude(op => op.MediaAssets));
        ApplyOrderByDescending(p => p.CreatedAt);
        ApplyTake(size+1);
    }
}