using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class InsurancePolicyConfiguration : IEntityTypeConfiguration<InsurancePolicy>
    {
        public void Configure(EntityTypeBuilder<InsurancePolicy> builder)
        {
            builder.ToTable("InsurancePolicies");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.PolicyNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(i => i.PolicyNumber)
                .IsUnique()
                .HasDatabaseName("IX_InsurancePolicies_PolicyNumber");

            builder.Property(i => i.CoverageAmount)
                .IsRequired()
                .HasColumnType("decimal(12,2)");

            builder.Property(i => i.PremiumAmount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(i => i.ExpiryDate)
                .IsRequired();

            builder.Property(i => i.IsClaimed)
                .HasDefaultValue(false);

            builder.Property(i => i.ClaimReason)
                .HasMaxLength(500);

            builder.Property(i => i.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
