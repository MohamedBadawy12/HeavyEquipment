using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class RentalOrderConfiguration : IEntityTypeConfiguration<RentalOrder>
    {
        public void Configure(EntityTypeBuilder<RentalOrder> builder)
        {
            builder.HasKey(ro => ro.Id);

            builder.Property(ro => ro.TotalPrice).HasColumnType("decimal(18,2)");

            builder.HasOne(ro => ro.Customer)
                   .WithMany(u => u.MyRentals)
                   .HasForeignKey(ro => ro.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ro => ro.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);
        }
    }
}
