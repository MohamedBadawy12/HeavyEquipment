using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class LogisticsProviderConfiguration : IEntityTypeConfiguration<LogisticsProvider>
    {
        public void Configure(EntityTypeBuilder<LogisticsProvider> builder)
        {
            builder.HasKey(lp => lp.Id);

            builder.Property(lp => lp.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(lp => lp.RatePerKilometer).HasColumnType("decimal(18,2)");


            builder.HasMany<RentalOrder>()
                   .WithOne(ro => ro.LogisticsProvider)
                   .HasForeignKey(ro => ro.LogisticsProviderId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
