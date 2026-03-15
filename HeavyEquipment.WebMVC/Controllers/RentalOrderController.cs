using HeavyEquipment.Application.Features.RentalOrders.Commands.Cancel;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Confirm;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Create;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    [Authorize]
    public class RentalOrderController : Controller
    {
        private readonly IMediator _mediator;

        public RentalOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


        public async Task<IActionResult> MyOrders()
        {
            var result = await _mediator.Send(
                new GetCustomerOrdersQuery(CurrentUserId));

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction("Index", "Home");
            }
            return View(result.Value);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _mediator.Send(new GetRentalOrderByIdQuery(id));

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction(nameof(MyOrders));
            }

            return View(result.Value);
        }

        [Authorize(Roles = "Customer")]
        public IActionResult Create(Guid equipmentId)
        {
            ViewBag.EquipmentId = equipmentId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRentalOrderCommand command)
        {
            if (!ModelState.IsValid)
                return View(command);

            var commandWithCustomer = command with { CustomerId = CurrentUserId };

            var result = await _mediator.Send(commandWithCustomer);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(command);
            }

            TempData["Success"] = "تم إرسال طلب الإيجار بنجاح! سيتم التواصل معك قريباً.";
            return RedirectToAction(nameof(Details), new { id = result.Value });
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var result = await _mediator.Send(new ConfirmRentalOrderCommand(id));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم تأكيد الطلب بنجاح!" : result.Error;

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsActive(Guid id)
        {
            var result = await _mediator.Send(new MarkOrderAsActiveCommand(id));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم تسليم المعدة وبدأ التأجير!" : result.Error;

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Owner")]
        public IActionResult Complete(Guid id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id, int hoursOperated)
        {
            var result = await _mediator.Send(
                new CompleteRentalOrderCommand(id, hoursOperated));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم إتمام الطلب بنجاح!" : result.Error;

            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Cancel(Guid id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, string reason)
        {
            var result = await _mediator.Send(new CancelRentalOrderCommand(id, reason));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم إلغاء الطلب." : result.Error;

            return RedirectToAction(nameof(MyOrders));
        }
    }
}
