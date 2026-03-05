using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;


namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class InsurancePolicyRepository : GenericRepository<InsurancePolicy>, IInsurancePolicyRepository
    {
        public InsurancePolicyRepository(AppDbContext context) : base(context) { }

        public async Task<InsurancePolicy?> GetByRentalOrderIdAsync(
            Guid rentalOrderId, CancellationToken ct = default) =>
             await _dbSet.FirstOrDefaultAsync(i => i.RentalOrderId == rentalOrderId, ct);

    }
}
