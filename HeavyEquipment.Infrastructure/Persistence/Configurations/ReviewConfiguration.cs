using HeavyEquipment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeavyEquipment.Infrastructure.Persistence.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5"));

            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");


            builder.HasIndex(r => new { r.RentalOrderId, r.ReviewerId, r.Type })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_UniqueReviewerPerOrder");

            builder.HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
