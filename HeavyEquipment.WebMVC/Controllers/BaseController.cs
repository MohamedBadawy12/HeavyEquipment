using HeavyEquipment.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class BaseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseController(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var unread = _unitOfWork.Notifications
                    .GetUnreadByUserIdAsync(userId).Result;
                ViewBag.UnreadCount = unread.Count;
            }
            base.OnActionExecuting(context);
        }
    }
}
