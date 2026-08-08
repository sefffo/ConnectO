using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {

        builder.HasIndex(c => new
        {
            c.PostId,
            c.CreatedAt
        }).IsDescending(false,false);
        
        // Fetch replies to a specific comment, oldest first
        builder.HasIndex(c => new { c.ParentCommentId, c.CreatedAt })
            .IsDescending(false, false);
        
        
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Content).HasMaxLength(1500);
        
        
        builder.HasOne(c => c.Post)
            .WithMany(p=>p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade); // on deleting the post, the comment is deleted too
        
        builder.HasOne(c => c.Author)
            .WithMany(u=>u.Comments)
            .HasForeignKey(c=>c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}