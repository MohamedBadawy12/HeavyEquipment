using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.RentalEvents;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Notifications.EventHandlers
{
    public class RentalCompletedEventHandler : INotificationHandler<RentalCompletedEvent>
    {
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RentalCompletedEventHandler(
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
        public async Task Handle(RentalCompletedEvent notification, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(notification.RentalOrderId, cancellationToken);
            if (order is null) return;

            // Customer notification
            await _notifications.SendAsync(
                notification.CustomerId,
                "اكتمل الإيجار 🏁",
                $"تم إتمام طلب الإيجار بنجاح. إجمالي المبلغ: {notification.TotalPrice:N0} ج.م",
                NotificationType.RentalCompleted,
                    notification.RentalOrderId);

            // Owner notification
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(order.EquipmentId, cancellationToken);
            if (equipment != null)
            {
                await _notifications.SendAsync(
                    equipment.OwnerId,
                    "تم إتمام إيجار معدتك 💰",
                    $"تم إتمام إيجار {equipment.Name} بنجاح. الإيرادات: {notification.TotalPrice:N0} ج.م",
                    NotificationType.RentalCompleted,
                    notification.RentalOrderId);
            }

            // Email للـ Customer
            var customer = await _userManager.FindByIdAsync(notification.CustomerId.ToString());
            if (customer?.Email != null)
            {
                await _email.SendAsync(
                    customer.Email,
                    customer.FullName,
                    "اكتمل إيجارك — HeavyHub",
                    BuildCompletedEmailHtml(customer.FullName, notification.RentalOrderId, notification.TotalPrice));
            }
        }
        private static string BuildCompletedEmailHtml(string name, Guid orderId, decimal total) => $"""
            <div dir="rtl" style="font-family:Cairo,sans-serif;max-width:600px;margin:auto;background:#111318;color:#F0EDE8;padding:2rem;border-radius:8px">
                <h2 style="color:#D4A843">🏁 اكتمل إيجارك</h2>
                <p>مرحباً <strong>{name}</strong>،</p>
                <p>تم إتمام طلب الإيجار بنجاح.</p>
                <p>إجمالي المبلغ: <strong style="color:#D4A843">{total:N0} ج.م</strong></p>
                <p style="color:#7A7D85;font-size:0.85rem">رقم الطلب: {orderId}</p>
                <hr style="border-color:#2A2D35"/>
                <p style="color:#7A7D85;font-size:0.8rem">فريق HeavyHub</p>
            </div>
            """;

    }
}
