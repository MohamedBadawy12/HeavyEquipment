using HeavyEquipment.Domain.Entities;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IReadOnlyList<Review>> GetByRentalOrderIdAsync(Guid rentalOrderId,
            CancellationToken ct = default);
        Task<double> GetAverageRatingAsync(Guid userId, CancellationToken ct = default);
    }
}
