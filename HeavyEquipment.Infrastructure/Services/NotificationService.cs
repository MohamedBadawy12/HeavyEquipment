using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;

namespace HeavyEquipment.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;
        public async Task SendAsync(Guid userId, string title, string message,
            NotificationType type, Guid? relatedEntityId = null)
        {
            var notification = new Notification(userId, title, message, type, relatedEntityId);
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
