using HeavyEquipment.Application.Features.Equipments.Commands.Create;
using HeavyEquipment.Application.Features.Equipments.Commands.Queries;
using HeavyEquipment.Application.Features.Equipments.Commands.Update;
using HeavyEquipment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IMediator _mediator;
        private Guid CurrentUserId =>
           Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public EquipmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(
            EquipmentCategory? category,
            string? city,
            DateTime? from,
            DateTime? to,
            int page = 1)
        {
            var query = new GetAvailableEquipmentsQuery(
                From: from ?? DateTime.UtcNow,
                To: to ?? DateTime.UtcNow.AddDays(1),
                Category: category,
                City: city,
                PageNumber: page,
                PageSize: 9);

            var result = await _mediator.Send(query);

            ViewBag.SelectedCategory = category;
            ViewBag.SelectedCity = city;
            ViewBag.From = from;
            ViewBag.To = to;

            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _mediator.Send(new GetEquipmentByIdQuery(id));

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        [Authorize(Roles = "Owner")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEquipmentCommand command)
        {
            if (!ModelState.IsValid)
                return View(command);

            var commandWithOwner = command with { OwnerId = CurrentUserId };

            var result = await _mediator.Send(commandWithOwner);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(command);
            }

            TempData["Success"] = "تم إضافة المعدة بنجاح!";
            return RedirectToAction(nameof(Details), new { id = result.Value });
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> MyEquipments()
        {
            var result = await _mediator.Send(
                new GetEquipmentsByOwnerQuery(CurrentUserId));

            return View(result);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid equipmentId, EquipmentStatus newStatus)
        {
            var result = await _mediator.Send(
                new UpdateEquipmentStatusCommand(equipmentId, newStatus));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم تحديث حالة المعدة" : result.Error;

            return RedirectToAction(nameof(MyEquipments));
        }
    }
}
