using HeavyEquipment.Domain.Entities;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Equipment> Equipments { get; }
        IGenericRepository<RentalOrder> RentalOrders { get; }
        IGenericRepository<AppUser> Users { get; }

        Task<int> CompleteAsync();
    }
}
