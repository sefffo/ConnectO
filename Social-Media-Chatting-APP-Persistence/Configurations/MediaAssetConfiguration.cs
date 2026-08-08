using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Uploader)
            .WithMany()
            .HasForeignKey(m => m.UploaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Message)
            .WithMany()
            .HasForeignKey(m => m.MessageId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(m => m.Conversation)
            .WithMany()
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(m=>m.Post)
            .WithMany(p=>p.MediaAssets)
            .HasForeignKey(m=>m.PostId)
            .OnDelete(DeleteBehavior.SetNull);// on deleting the post, the media asset is deleted too
        
        builder.HasOne(m=>m.Comment)
            .WithMany(c=>c.MediaAssets)
            .HasForeignKey(m=>m.CommentId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(m => m.Url).IsRequired();
        builder.Property(m => m.PublicId).IsRequired();
        builder.Property(m => m.OriginalFileName).IsRequired();
    }
}