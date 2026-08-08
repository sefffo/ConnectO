using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {

        builder.HasIndex(p =>new { p.AuthorId , p.CreatedAt}).IsDescending(false,true);
        builder.HasIndex(p => p.OriginalPostId);
        
        builder.Property(p => p.PostType).ToString();
        builder.Property(p => p.Content).HasMaxLength(3000);
        builder.Property(p => p.QuoteContent).HasMaxLength(1000);
        //self-referencing nav prop for the repost
        builder.HasOne(p=>p.OriginalPost)
            .WithMany(p=>p.Reposts)
            .HasForeignKey(p=>p.OriginalPostId)
            .OnDelete(DeleteBehavior.Restrict); // on deleting the post, the repost is deleted too
        // configure all the relations manually as a test for myself 
        builder.HasOne(p=>p.Author).WithMany(u=>u.Posts)
            .HasForeignKey(p=>p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict); // on deleting the user, the post is deleted too
        
    }
}