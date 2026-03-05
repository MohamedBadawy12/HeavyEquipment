using HeavyEquipment.Domain.Entities;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface ILogisticsProviderRepository : IGenericRepository<LogisticsProvider>
    {
        Task<IReadOnlyList<LogisticsProvider>> GetActiveProvidersAsync(
            CancellationToken ct = default);
    }
}
