using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class EquipmentRepository : GenericRepository<Equipment>, IEquipmentRepository
    {
        public EquipmentRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Equipment>> GetAvailableEquipmentsAsync(DateTime from, DateTime to,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(e => e.Status == EquipmentStatus.Available)
                .Where(e => !e.Rentals.Any(r =>
                    r.Status != OrderStatus.Cancelled &&
                    r.RentalStart < to &&
                    r.RentalEnd > from))
                .Include(e => e.Owner)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Equipment>> GetByCategoryAsync(EquipmentCategory category,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(e => e.Category == category && e.Status == EquipmentStatus.Available)
                .Include(e => e.Owner)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Equipment>> GetByOwnerIdAsync(Guid ownerId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(e => e.OwnerId == ownerId)
                .Include(e => e.Rentals)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Equipment>> GetEquipmentsDueForMaintenanceAsync(
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(e => (e.NextMaintenanceThreshold - e.TotalHoursOperated) <= 10)
                .Include(e => e.Owner)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Equipment>> GetNearbyEquipmentsAsync(double latitude, double longitude,
            double radiusInKm, CancellationToken ct = default)
        {
            var equipments = await _dbSet
                .Where(e => e.Status == EquipmentStatus.Available)
                .Include(e => e.Owner)
                .ToListAsync(ct);

            return equipments
                .Where(e => e.CurrentLocation.DistanceTo(
                    new Domain.ValueObjects.Location("", "", latitude, longitude)) <= radiusInKm)
                .ToList();
        }
    }
}
