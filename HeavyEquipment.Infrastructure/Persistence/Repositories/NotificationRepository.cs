using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
            Guid userId, CancellationToken ct = default)
        {
            return await _dbSet.Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
        {
            var notifications = await _dbSet.Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(ct);

            notifications.ForEach(n => n.MarkAsRead());
        }
    }
}
