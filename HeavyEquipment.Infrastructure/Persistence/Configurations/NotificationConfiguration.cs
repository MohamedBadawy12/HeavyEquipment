using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(n => new { n.UserId, n.IsRead })
                .HasDatabaseName("IX_Notifications_UserId_IsRead");
        }
    }
}
