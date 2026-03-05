using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Review>> GetByRentalOrderIdAsync(
            Guid rentalOrderId, CancellationToken ct = default) =>
             await _dbSet.Where(r => r.RentalOrderId == rentalOrderId)
                .Include(r => r.Reviewer)
                .ToListAsync(ct);


        public async Task<double> GetAverageRatingAsync(
            Guid userId, CancellationToken ct = default)
        {
            var reviews = await _dbSet.Where(r => r.ReviewerId == userId)
                .ToListAsync(ct);

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }
    }
}
