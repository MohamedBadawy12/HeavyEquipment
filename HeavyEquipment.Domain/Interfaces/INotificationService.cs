using HeavyEquipment.Domain.Enums;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(
            Guid userId,
            string title,
            string message,
            NotificationType type,
            Guid? relatedEntityId = null);
    }
}
