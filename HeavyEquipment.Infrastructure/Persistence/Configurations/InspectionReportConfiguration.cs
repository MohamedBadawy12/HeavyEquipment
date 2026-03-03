using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class InspectionReportConfiguration : IEntityTypeConfiguration<InspectionReport>
    {
        public void Configure(EntityTypeBuilder<InspectionReport> builder)
        {
            builder.HasKey(ir => ir.Id);

            builder.Property(ir => ir.PhotoUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            ).HasColumnType("nvarchar(max)"); ;

            builder.Property(ir => ir.Result).HasConversion<string>();

            builder.HasOne(ir => ir.RentalOrder)
                   .WithMany(ro => ro.Inspections)
                   .HasForeignKey(ir => ir.RentalOrderId);
        }
    }
}
