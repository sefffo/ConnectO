using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.Posts;

public class USerPostSpecifications : BaseSpecification<Post>
{
    public USerPostSpecifications(string userId, DateTime cursor, int size) : base(p =>
        p.IsDeleted == false && p.AuthorId == userId && (cursor == null || p.CreatedAt < cursor))
    {
        ApplyOrderByDescending(p => p.CreatedAt);
        ApplyTake(size);
    }
}