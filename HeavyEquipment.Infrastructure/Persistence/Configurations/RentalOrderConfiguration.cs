using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class RentalOrderConfiguration : IEntityTypeConfiguration<RentalOrder>
    {
        public void Configure(EntityTypeBuilder<RentalOrder> builder)
        {
            builder.ToTable("RentalOrders");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.HourlyRate)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(r => r.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(12,2)");

            builder.Property(r => r.RentalStart)
                .IsRequired();

            builder.Property(r => r.RentalEnd)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(r => r.CancellationReason)
                .HasMaxLength(500);

            builder.Property(r => r.DisputeReason)
                .HasMaxLength(500);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(r => r.CustomerId)
                .HasDatabaseName("IX_RentalOrders_CustomerId");

            builder.HasIndex(r => r.EquipmentId)
                .HasDatabaseName("IX_RentalOrders_EquipmentId");

            builder.HasIndex(r => r.Status)
                .HasDatabaseName("IX_RentalOrders_Status");

            builder.HasIndex(r => new { r.EquipmentId, r.RentalStart, r.RentalEnd })
                .HasDatabaseName("IX_RentalOrders_Availability");

            builder.HasOne(r => r.Insurance)
                .WithOne(i => i.RentalOrder)
                .HasForeignKey<InsurancePolicy>(i => i.RentalOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Inspections)
                .WithOne(i => i.RentalOrder)
                .HasForeignKey(i => i.RentalOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Reviews)
                .WithOne(rv => rv.RentalOrder)
                .HasForeignKey(rv => rv.RentalOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.LogisticsProvider)
                .WithMany()
                .HasForeignKey(r => r.LogisticsProviderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
