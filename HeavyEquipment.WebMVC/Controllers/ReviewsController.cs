using HeavyEquipment.Application.Features.Reviews.Commands.Create;
using HeavyEquipment.Application.Features.Reviews.Commands.Queries;
using HeavyEquipment.Application.Features.Reviews.Commands.Update;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class ReviewsController : BaseController
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return RedirectToAction("Details", "RentalOrder", new { id = command.RentalOrderId });

            TempData["Error"] = result.Error;
            return RedirectToAction("Details", "RentalOrder", new { id = command.RentalOrderId });
        }

        [HttpGet]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var result = await _mediator.Send(new GetReviewsByOrderQuery(orderId));
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateReviewCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["Success"] = "تم تحديث التعليق بنجاح.";
                return RedirectToAction("Details", "RentalOrder");
            }

            TempData["Error"] = result.Error;
            return RedirectToAction("Details", "RentalOrder");
        }
    }
}
