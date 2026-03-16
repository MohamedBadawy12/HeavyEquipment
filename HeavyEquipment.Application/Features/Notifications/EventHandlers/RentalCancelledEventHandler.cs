using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.RentalEvents;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Notifications.EventHandlers
{
    public class RentalCancelledEventHandler : INotificationHandler<RentalCancelledEvent>
    {
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RentalCancelledEventHandler(
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
        public async Task Handle(RentalCancelledEvent notification, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(notification.RentalOrderId, cancellationToken);
            if (order is null) return;

            // Customer notification
            await _notifications.SendAsync(
                notification.CustomerId,
                "تم إلغاء الطلب",
                $"تم إلغاء طلب الإيجار. السبب: {notification.Reason}",
                NotificationType.RentalCancelled,
                notification.RentalOrderId);

            // Owner notification
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(order.EquipmentId, cancellationToken);
            if (equipment != null)
            {
                await _notifications.SendAsync(
                    equipment.OwnerId,
                    "تم إلغاء طلب إيجار معدتك",
                    $"تم إلغاء طلب إيجار {equipment.Name}. السبب: {notification.Reason}",
                    NotificationType.RentalCancelled,
                    notification.RentalOrderId);
            }

            // Email للـ Customer
            var customer = await _userManager.FindByIdAsync(notification.CustomerId.ToString());
            if (customer?.Email != null)
            {
                await _email.SendAsync(
                    customer.Email,
                    customer.FullName,
                    "تم إلغاء طلب الإيجار — HeavyHub",
                    BuildCancelledEmailHtml(customer.FullName, notification.RentalOrderId, notification.Reason));
            }
        }
        private static string BuildCancelledEmailHtml(string name, Guid orderId, string reason) => $"""
            <div dir="rtl" style="font-family:Cairo,sans-serif;max-width:600px;margin:auto;background:#111318;color:#F0EDE8;padding:2rem;border-radius:8px">
                <h2 style="color:#C0392B">❌ تم إلغاء الطلب</h2>
                <p>مرحباً <strong>{name}</strong>،</p>
                <p>تم إلغاء طلب الإيجار.</p>
                <p>السبب: <strong>{reason}</strong></p>
                <p style="color:#7A7D85;font-size:0.85rem">رقم الطلب: {orderId}</p>
                <hr style="border-color:#2A2D35"/>
                <p style="color:#7A7D85;font-size:0.8rem">فريق HeavyHub</p>
            </div>
            """;
    }
}
