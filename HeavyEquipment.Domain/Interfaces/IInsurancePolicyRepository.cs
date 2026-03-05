using HeavyEquipment.Domain.Entities;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IInsurancePolicyRepository : IGenericRepository<InsurancePolicy>
    {
        Task<InsurancePolicy?> GetByRentalOrderIdAsync(Guid rentalOrderId,
            CancellationToken ct = default);
    }
}
