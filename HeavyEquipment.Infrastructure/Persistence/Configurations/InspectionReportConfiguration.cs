using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class InspectionReportConfiguration : IEntityTypeConfiguration<InspectionReport>
    {
        public void Configure(EntityTypeBuilder<InspectionReport> builder)
        {
            builder.ToTable("InspectionReports");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Notes)
                .HasMaxLength(1000);

            builder.Property(i => i.HoursReading)
                .IsRequired();

            builder.Property(i => i.Result)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(i => i.InspectionDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.PhotoUrls)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .HasColumnName("PhotoUrls")
                .HasMaxLength(4000);

            builder.HasIndex(i => i.RentalOrderId)
                .HasDatabaseName("IX_InspectionReports_RentalOrderId");
        }
    }
}
