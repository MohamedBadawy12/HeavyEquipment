using HeavyEquipment.Application.Features.Inspections.Commands;
using HeavyEquipment.Application.Features.Insurances.Commands;
using HeavyEquipment.Application.Features.Logistics.Commands;
using HeavyEquipment.Application.Features.Logistics.Dtos;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class OrderExtrasController : BaseController
    {
        private readonly IMediator _mediator;
        public OrderExtrasController(IMediator mediator, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mediator = mediator;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: OrderExtras/AddInsurance/orderId
        [Authorize(Roles = "Customer")]
        public IActionResult AddInsurance(Guid orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        // POST: OrderExtras/AddInsurance
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInsurance(
            Guid orderId, decimal coverageAmount,
            decimal premiumAmount, DateTime expiryDate)
        {
            var result = await _mediator.Send(
                new AddInsuranceCommand(orderId, coverageAmount, premiumAmount, expiryDate));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم إضافة التأمين بنجاح ✅" : result.Error;

            return RedirectToAction("Details", "RentalOrder", new { id = orderId });
        }

        // GET: OrderExtras/AddInspection/orderId
        [Authorize(Roles = "Owner")]
        public IActionResult AddInspection(Guid orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        // POST: OrderExtras/AddInspection
        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInspection(
            Guid orderId, InspectionType type,
            InspectionStatus result, int hoursReading, string notes)
        {
            var cmd = await _mediator.Send(
                new AddInspectionCommand(orderId, CurrentUserId, type, result, hoursReading, notes));

            TempData[cmd.IsSuccess ? "Success" : "Error"] =
                cmd.IsSuccess ? "تم إضافة تقرير الفحص بنجاح ✅" : cmd.Error;

            return RedirectToAction("Details", "RentalOrder", new { id = orderId });
        }


        // GET: OrderExtras/AssignLogistics/orderId
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AssignLogistics(Guid orderId)
        {
            var result = await _mediator.Send(new GetLogisticsProvidersQuery());
            ViewBag.OrderId = orderId;
            ViewBag.Providers = result.IsSuccess ? result.Value : new List<LogisticsProviderDto>();
            return View();
        }

        // POST: OrderExtras/AssignLogistics
        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignLogistics(Guid orderId, Guid providerId)
        {
            var result = await _mediator.Send(new AssignLogisticsCommand(orderId, providerId));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم تعيين شركة النقل بنجاح ✅" : result.Error;

            return RedirectToAction("Details", "RentalOrder", new { id = orderId });
        }
    }
}
