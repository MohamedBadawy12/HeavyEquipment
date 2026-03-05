using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class LogisticsProviderRepository : GenericRepository<LogisticsProvider>, ILogisticsProviderRepository
    {
        public LogisticsProviderRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<LogisticsProvider>> GetActiveProvidersAsync(
            CancellationToken ct = default) =>
             await _dbSet.Where(l => l.IsActive).ToListAsync(ct);

    }
}
