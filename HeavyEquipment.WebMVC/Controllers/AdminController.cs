using HeavyEquipment.Application.Features.Users.Commands;
using HeavyEquipment.Application.Features.Users.Commands.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;
        public AdminController(IMediator mediator) => _mediator = mediator;

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAdminDashboardQuery());
            if (!result.IsSuccess) return View("Error");
            return View(result.Value);
        }

        public async Task<IActionResult> Users(string? role, string? search)
        {
            var result = await _mediator.Send(new GetAdminUsersQuery(role, search));
            if (!result.IsSuccess) return View("Error");
            return View(result.Value);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyUser(Guid userId)
        {
            var result = await _mediator.Send(new VerifyUserCommand(userId));
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم توثيق المستخدم بنجاح ✅" : result.Error;
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(Guid userId)
        {
            var result = await _mediator.Send(new BlockUserCommand(userId));
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم حظر المستخدم" : result.Error;
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(Guid userId)
        {
            var result = await _mediator.Send(new UnblockUserCommand(userId));
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم إلغاء الحظر ✅" : result.Error;
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Equipments(string? category, string? status)
        {
            var result = await _mediator.Send(new GetAdminEquipmentsQuery(category, status));
            if (!result.IsSuccess) return View("Error");
            return View(result.Value);
        }

        public async Task<IActionResult> Orders(string? status)
        {
            var result = await _mediator.Send(new GetAdminOrdersQuery(status));
            if (!result.IsSuccess) return View("Error");
            return View(result.Value);
        }
    }
}
