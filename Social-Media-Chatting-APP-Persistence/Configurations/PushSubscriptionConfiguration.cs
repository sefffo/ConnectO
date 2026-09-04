using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(450); // Identity default max for string IDs

        builder.Property(p => p.Endpoint)
            .IsRequired()
            .HasMaxLength(2048); // push endpoints can be long URLs

        builder.Property(p => p.P256dh)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Auth)
            .IsRequired()
            .HasMaxLength(128);

        // Unique index on Endpoint — one browser = one row, makes subscribe idempotent
        // Also ensures we don't duplicate-deliver to the same browser
        builder.HasIndex(p => p.Endpoint).IsUnique();

        // Index on UserId for fast lookup: "get all subscriptions for user X"
        builder.HasIndex(p => p.UserId);

        // AppUser (string PK) -> PushSubscription (string FK)
        // Restrict: don't delete subscriptions when user is deleted, handle in app layer
        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade); // cascade is fine: no user = no push targets
    }
}
