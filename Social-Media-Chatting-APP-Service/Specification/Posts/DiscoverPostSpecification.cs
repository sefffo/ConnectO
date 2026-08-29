using Microsoft.EntityFrameworkCore;
using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Posts;

/// <summary>
/// Fetches posts from friends-of-friends for the Discover section.
///
/// Exclusions:
///   - Posts by the current user
///   - Posts by direct friends (already in the Friends feed)
///   - Posts by blocked users (anyone the current user blocked or was blocked by)
///
/// Sorted by popularity score (likes + comments) descending so the
/// discover section surfaces interesting content rather than just recent noise.
/// Cursor here is based on a composite popularity score stored as a double
/// but since BaseSpecification supports only DateTime cursor natively,
/// we use a separate int-based skip approach via ApplyTake only —
/// the handler will manage the discover cursor via page index passed as int.
/// </summary>
public class DiscoverPostSpecification : BaseSpecification<Post>
{
   public DiscoverPostSpecification(
      List<string> discoverAuthorIds,
      List<string> excludedIds,
      int size,
      int skip =0
      ) : base( p=>discoverAuthorIds.Contains(p.AuthorId)&&!excludedIds.Contains(p.AuthorId) && p.IsDeleted == false)
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
      ApplyTake(size*3+skip); // over take 3shan hn3ml filter in the handler mem
   }
}
