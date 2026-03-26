using HeavyEquipment.Application.Features.Reviews.Commands.Create;
using HeavyEquipment.Application.Features.Reviews.Commands.Queries;
using HeavyEquipment.Application.Features.Reviews.Commands.Update;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class ReviewsController : BaseController
    {
        private readonly IMediator _mediator;
        public ReviewsController(IMediator mediator, IUnitOfWork unitOfWork) : base(unitOfWork) => _mediator = mediator;

        private Guid CurrentUserId =>
           Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


        [Authorize(Roles = "Customer")]
        public IActionResult Create(Guid orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Guid orderId, int rating, string comment)
        {
            var result = await _mediator.Send(new CreateReviewCommand(
                orderId, CurrentUserId, rating, comment, ReviewType.RenterReview));

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "تم إضافة تقييمك بنجاح ✅" : result.Error;

            return RedirectToAction("Details", "RentalOrder", new { id = orderId });
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
