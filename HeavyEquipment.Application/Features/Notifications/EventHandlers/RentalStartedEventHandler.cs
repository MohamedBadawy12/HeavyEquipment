using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.RentalEvents;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Notifications.EventHandlers
{
    public class RentalStartedEventHandler : INotificationHandler<RentalStartedEvent>
    {
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RentalStartedEventHandler(
            INotificationService notifications,
            IEmailService email,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _notifications = notifications;
            _email = email;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task Handle(RentalStartedEvent notification, CancellationToken cancellationToken)
        {
            // جيب الـ Order عشان نعرف الـ CustomerId
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(notification.RentalOrderId, cancellationToken);
            if (order is null) return;

            await _notifications.SendAsync(
                order.CustomerId,
                "بدأ الإيجار 🚀",
                "المعدة الآن في حوزتك. نتمنى لك مشروعاً ناجحاً!",
                NotificationType.RentalStarted,
                notification.RentalOrderId);

            var customer = await _userManager.FindByIdAsync(order.CustomerId.ToString());
            if (customer?.Email != null)
            {
                await _email.SendAsync(
                    customer.Email,
                    customer.FullName,
                    "بدأ إيجارك — HeavyHub",
                    BuildStartedEmailHtml(customer.FullName, notification.RentalOrderId));
            }
        }
        private static string BuildStartedEmailHtml(string name, Guid orderId) => $"""
            <div dir="rtl" style="font-family:Cairo,sans-serif;max-width:600px;margin:auto;background:#111318;color:#F0EDE8;padding:2rem;border-radius:8px">
                <h2 style="color:#D4A843">🚀 بدأ إيجارك</h2>
                <p>مرحباً <strong>{name}</strong>،</p>
                <p>المعدة الآن في حوزتك. نتمنى لك مشروعاً ناجحاً!</p>
                <p style="color:#7A7D85;font-size:0.85rem">رقم الطلب: {orderId}</p>
                <hr style="border-color:#2A2D35"/>
                <p style="color:#7A7D85;font-size:0.8rem">فريق HeavyHub</p>
            </div>
            """;
    }
}
