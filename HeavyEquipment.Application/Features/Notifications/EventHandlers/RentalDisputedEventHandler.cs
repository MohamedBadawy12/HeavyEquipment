using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.RentalEvents;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Notifications.EventHandlers
{
    public class RentalDisputedEventHandler : INotificationHandler<RentalDisputedEvent>
    {
        private readonly INotificationService _notifications;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RentalDisputedEventHandler(
            INotificationService notifications,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _notifications = notifications;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task Handle(RentalDisputedEvent notification, CancellationToken cancellationToken)
        {
            // خبر كل الـ Admins
            var admins = _userManager.Users
                .Where(u => u.Role == HeavyEquipment.Domain.Enums.UserType.Admin)
                .ToList();

            foreach (var admin in admins)
            {
                await _notifications.SendAsync(
                    admin.Id,
                    "نزاع جديد يحتاج مراجعة ⚠️",
                    $"تم رفع نزاع على طلب إيجار. السبب: {notification.Reason}",
                    NotificationType.MaintenanceAlert,
                    notification.RentalOrderId);
            }
        }
    }
}
