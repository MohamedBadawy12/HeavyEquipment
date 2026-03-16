using HeavyEquipment.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<IActionResult> Index()
        {
            var notifications = await _unitOfWork.Notifications
                .GetUnreadByUserIdAsync(CurrentUserId);
            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _unitOfWork.Notifications.MarkAllAsReadAsync(CurrentUserId);
            TempData["Success"] = "تم تحديد كل الإشعارات كمقروءة";
            return RedirectToAction(nameof(Index));
        }
    }
}
