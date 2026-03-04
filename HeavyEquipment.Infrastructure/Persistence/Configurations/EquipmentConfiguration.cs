using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.ToTable("Equipments");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.ManufactureYear)
                .IsRequired();

            builder.Property(e => e.Category)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(e => e.HourlyRate)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(e => e.DepositAmount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(e => e.TotalHoursOperated)
                .HasDefaultValue(0);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            builder.OwnsOne(e => e.CurrentLocation, location =>
            {
                location.Property(l => l.City)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("City");

                location.Property(l => l.Address)
                    .HasMaxLength(300)
                    .HasColumnName("Address");

                location.Property(l => l.Latitude)
                    .IsRequired()
                    .HasColumnName("Latitude");

                location.Property(l => l.Longitude)
                    .IsRequired()
                    .HasColumnName("Longitude");
            });

            builder.Property(e => e.PhotoUrls)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .HasColumnName("PhotoUrls")
                .HasMaxLength(4000);

            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasIndex(e => e.OwnerId)
                .HasDatabaseName("IX_Equipments_OwnerId");

            builder.HasIndex(e => e.Category)
                .HasDatabaseName("IX_Equipments_Category");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Equipments_Status");

            builder.HasMany(e => e.Rentals)
               .WithOne(r => r.Equipment)
               .HasForeignKey(r => r.EquipmentId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
