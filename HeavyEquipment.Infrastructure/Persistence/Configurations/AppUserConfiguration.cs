using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.FullName)
                 .IsRequired()
                 .HasMaxLength(150);

            builder.Property(u => u.NationalId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.TrustScore)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(100);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.IsVerified)
                .HasDefaultValue(false);

            builder.Property(u => u.IsBlocked)
                .HasDefaultValue(false);

            builder.Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            builder.HasQueryFilter(u => !u.IsDeleted);

            builder.HasIndex(u => u.NationalId)
                .IsUnique()
                .HasDatabaseName("IX_Users_NationalId");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasMany(u => u.OwnedEquipments)
              .WithOne(e => e.Owner)
              .HasForeignKey(e => e.OwnerId)
              .OnDelete(DeleteBehavior.Restrict);

            // User → MyRentals (One to Many)
            builder.HasMany(u => u.MyRentals)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Notifications (One to Many)
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
