using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(n => n.Recipient)
                .WithMany()
                .HasForeignKey(n => n.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Actor)
                .WithMany()
                .HasForeignKey(n => n.ActorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(n => n.Type)
                .HasConversion<string>();

            builder.HasIndex(n => new { n.RecipientId, n.CreatedAt });
            builder.HasIndex(n => new { n.RecipientId, n.IsRead });
        }
    }
}
