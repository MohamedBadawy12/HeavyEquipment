using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired().HasMaxLength(150);

            builder.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");

            //builder.OwnsOne(e => e.CurrentLocation, loc =>
            //{
            //    loc.Property(l => l.AddressLine).HasColumnName("Address");
            //    loc.Property(l => l.Latitude).HasColumnName("Latitude");
            //    loc.Property(l => l.Longitude).HasColumnName("Longitude");
            //});

            builder.HasOne(e => e.Owner)
                   .WithMany(u => u.OwnedEquipments)
                   .HasForeignKey(e => e.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
