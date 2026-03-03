using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class InsurancePolicyConfiguration : IEntityTypeConfiguration<InsurancePolicy>
    {
        public void Configure(EntityTypeBuilder<InsurancePolicy> builder)
        {
            builder.HasKey(ip => ip.Id);
            builder.Property(ip => ip.PolicyNumber).IsRequired().HasMaxLength(50);
            builder.Property(ip => ip.CoverageAmount).HasColumnType("decimal(18,2)");

            builder.HasOne(ip => ip.RentalOrder)
                   .WithOne(ro => ro.Insurance)
                   .HasForeignKey<InsurancePolicy>(ip => ip.RentalOrderId);
        }
    }
}
