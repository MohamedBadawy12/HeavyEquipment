using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class RentalOrderRepository : GenericRepository<RentalOrder>, IRentalOrderRepository
    {
        public RentalOrderRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<RentalOrder>> GetActiveOrdersAsync(CancellationToken ct = default)
        {
            return await _dbSet
            .Where(r => r.Status == OrderStatus.Active
                     || r.Status == OrderStatus.Pending
                     || r.Status == OrderStatus.Confirmed)
            .Include(r => r.Equipment)
            .Include(r => r.Customer)
            .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<RentalOrder>> GetByCustomerIdAsync(Guid customerId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Equipment)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<RentalOrder>> GetByEquipmentIdAsync(Guid equipmentId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.EquipmentId == equipmentId)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<RentalOrder>> GetOverdueOrdersAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.Status == OrderStatus.Active && r.RentalEnd < DateTime.UtcNow)
                .Include(r => r.Equipment)
                .Include(r => r.Customer)
                .ToListAsync(ct);
        }

        public async Task<RentalOrder?> GetWithDetailsAsync(Guid orderId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(r => r.Equipment).ThenInclude(e => e.Owner)
                .Include(r => r.Customer)
                .Include(r => r.Insurance)
                .Include(r => r.Inspections)
                .Include(r => r.Reviews)
                .Include(r => r.LogisticsProvider)
                .FirstOrDefaultAsync(r => r.Id == orderId, ct);
        }

        public async Task<bool> IsEquipmentAvailableAsync(Guid equipmentId, DateTime from, DateTime to,
            CancellationToken ct = default)
        {
            return !await _dbSet.AnyAsync(r =>
                r.EquipmentId == equipmentId &&
                r.Status != OrderStatus.Cancelled &&
                r.RentalStart < to &&
                r.RentalEnd > from, ct);
        }

        public async Task<IReadOnlyList<RentalOrder>> GetByEquipmentOwnerIdAsync(
         Guid ownerId, CancellationToken ct = default)
        {
            return await _dbSet
                 .Where(r => r.Equipment!.OwnerId == ownerId)
                 .Include(r => r.Equipment)
                 .Include(r => r.Customer)
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync(ct);
        }
    }
}
