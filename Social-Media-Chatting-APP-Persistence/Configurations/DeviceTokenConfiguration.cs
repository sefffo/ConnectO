using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("DeviceTokens");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.UserId)
            .IsRequired()
            .HasMaxLength(450); // Identity default

        builder.Property(d => d.Token)
            .IsRequired()
            .HasMaxLength(512); // FCM tokens are typically ~152 chars but can grow

        builder.Property(d => d.DeviceName)
            .HasMaxLength(100);

        builder.Property(d => d.Platform)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("android");

        // Unique index on Token — one device = one row, makes register idempotent
        builder.HasIndex(d => d.Token).IsUnique();

        // Index on UserId for fast lookup: "get all tokens for user X"
        builder.HasIndex(d => d.UserId);

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade); // user deleted = tokens gone
    }
}
