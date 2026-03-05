using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class InspectionReportRepository : GenericRepository<InspectionReport>, IInspectionReportRepository
    {
        public InspectionReportRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<InspectionReport>> GetByRentalOrderIdAsync(
            Guid rentalOrderId, CancellationToken ct = default) =>
             await _dbSet.Where(i => i.RentalOrderId == rentalOrderId)
                .OrderBy(i => i.InspectionDate)
                .ToListAsync(ct);

    }
}
