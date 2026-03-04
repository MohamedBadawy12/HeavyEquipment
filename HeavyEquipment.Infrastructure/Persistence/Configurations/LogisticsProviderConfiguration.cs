using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class LogisticsProviderConfiguration : IEntityTypeConfiguration<LogisticsProvider>
    {
        public void Configure(EntityTypeBuilder<LogisticsProvider> builder)
        {
            builder.ToTable("LogisticsProviders");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.ContactNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(l => l.RatePerKilometer)
                .IsRequired()
                .HasColumnType("decimal(8,2)");

            builder.Property(l => l.IsActive)
                .HasDefaultValue(true);

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
