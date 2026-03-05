using HeavyEquipment.Domain.Entities;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IInspectionReportRepository : IGenericRepository<InspectionReport>
    {
        Task<IReadOnlyList<InspectionReport>> GetByRentalOrderIdAsync(Guid rentalOrderId
            , CancellationToken ct = default);
    }
}
