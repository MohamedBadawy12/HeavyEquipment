using HeavyEquipment.Application.Features.Logistics.Commands;
using HeavyEquipment.Application.Features.Users.Commands;
using HeavyEquipment.Application.Features.Users.Commands.Queries;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IMediator _mediator;
        public AdminController(IMediator mediator, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mediator = mediator;
        }

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

        public async Task<IActionResult> Logistics()
        {
            var result = await _mediator.Send(new GetAllLogisticsProvidersQuery());
            if (!result.IsSuccess) return View("Error");
            return View(result.Value);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLogistics(
            string companyName, string contactNumber, decimal ratePerKilometer)
        {
            var result = await _mediator.Send(
                new CreateLogisticsProviderCommand(companyName, contactNumber, ratePerKilometer));
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم إضافة شركة النقل بنجاح ✅" : result.Error;
            return RedirectToAction(nameof(Logistics));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLogistics(Guid id)
        {
            var result = await _mediator.Send(new ToggleLogisticsProviderCommand(id));
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم تحديث حالة شركة النقل ✅" : result.Error;
            return RedirectToAction(nameof(Logistics));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLogisticsRate(Guid id, decimal newRate)
        {
            var result = await _mediator.Send(new UpdateLogisticsRateCommand(id, newRate));
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم تحديث السعر بنجاح ✅" : result.Error;
            return RedirectToAction(nameof(Logistics));
        }
    }
}
