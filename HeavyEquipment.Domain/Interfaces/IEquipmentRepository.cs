using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IEquipmentRepository : IGenericRepository<Equipment>
    {
        Task<IReadOnlyList<Equipment>> GetAvailableEquipmentsAsync(DateTime from, DateTime to,
            CancellationToken ct = default);
        Task<IReadOnlyList<Equipment>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default);
        Task<IReadOnlyList<Equipment>> GetByCategoryAsync(EquipmentCategory category, CancellationToken ct = default);
        Task<IReadOnlyList<Equipment>> GetNearbyEquipmentsAsync(double latitude, double longitude, double radiusInKm,
           CancellationToken ct = default);
        Task<IReadOnlyList<Equipment>> GetEquipmentsDueForMaintenanceAsync(
           CancellationToken ct = default);
    }
}
