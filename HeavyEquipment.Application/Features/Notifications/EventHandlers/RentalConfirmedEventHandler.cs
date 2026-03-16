using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.RentalEvents;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Notifications.EventHandlers
{
    public class RentalConfirmedEventHandler : INotificationHandler<RentalConfirmedEvent>
    {
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RentalConfirmedEventHandler(
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
        public async Task Handle(RentalConfirmedEvent notification, CancellationToken cancellationToken)
        {
            // Notification للـ Customer
            await _notifications.SendAsync(
                notification.CustomerId,
                "تم تأكيد طلب الإيجار ✅",
                "تم تأكيد طلبك بنجاح. يمكنك الآن التواصل مع المالك لترتيب التسليم.",
                NotificationType.RentalConfirmed,
                notification.RentalOrderId);

            // Email للـ Customer
            var customer = await _userManager.FindByIdAsync(notification.CustomerId.ToString());
            if (customer?.Email != null)
            {
                await _email.SendAsync(
                    customer.Email,
                    customer.FullName,
                    "تم تأكيد طلب الإيجار — HeavyHub",
                    BuildConfirmedEmailHtml(customer.FullName, notification.RentalOrderId));
            }
        }
        private static string BuildConfirmedEmailHtml(string name, Guid orderId) => $"""
            <div dir="rtl" style="font-family:Cairo,sans-serif;max-width:600px;margin:auto;background:#111318;color:#F0EDE8;padding:2rem;border-radius:8px">
                <h2 style="color:#D4A843">✅ تم تأكيد طلب الإيجار</h2>
                <p>مرحباً <strong>{name}</strong>،</p>
                <p>تم تأكيد طلب الإيجار الخاص بك بنجاح.</p>
                <p style="color:#7A7D85;font-size:0.85rem">رقم الطلب: {orderId}</p>
                <hr style="border-color:#2A2D35"/>
                <p style="color:#7A7D85;font-size:0.8rem">فريق HeavyHub</p>
            </div>
            """;
    }
}
