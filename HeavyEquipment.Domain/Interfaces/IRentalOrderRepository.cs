using HeavyEquipment.Domain.Entities;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IRentalOrderRepository : IGenericRepository<RentalOrder>
    {
        Task<IReadOnlyList<RentalOrder>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
        Task<IReadOnlyList<RentalOrder>> GetByEquipmentIdAsync(Guid equipmentId, CancellationToken ct = default);
        Task<IReadOnlyList<RentalOrder>> GetActiveOrdersAsync(CancellationToken ct = default);
        Task<bool> IsEquipmentAvailableAsync(Guid equipmentId, DateTime from, DateTime to
            , CancellationToken ct = default);
        Task<IReadOnlyList<RentalOrder>> GetOverdueOrdersAsync(CancellationToken ct = default);
        Task<RentalOrder?> GetWithDetailsAsync(Guid orderId, CancellationToken ct = default);
        Task<IReadOnlyList<RentalOrder>> GetByEquipmentOwnerIdAsync(
            Guid ownerId, CancellationToken ct = default);
    }
}
