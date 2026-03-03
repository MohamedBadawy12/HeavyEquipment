using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }
        public virtual AppUser User { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; private set; } = false;
        public DateTime? ReadAt { get; private set; }
        public NotificationType Type { get; private set; }
        public Guid? RelatedEntityId { get; private set; }

        protected Notification() { }
        public Notification(Guid userId, string title, string message, NotificationType type, Guid? relatedEntityId = null)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new DomainException("عنوان الإشعار مطلوب");
            if (string.IsNullOrWhiteSpace(message)) throw new DomainException("نص الإشعار مطلوب");

            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            RelatedEntityId = relatedEntityId;
        }

        public void MarkAsRead()
        {
            if (IsRead) return;
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }
}
