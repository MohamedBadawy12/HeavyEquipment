using HeavyEquipment.Application.Features.Dashboard.Commands;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    [Authorize(Roles = "Owner,Admin")]
    public class DashboardController : BaseController
    {
        private readonly IMediator _mediator;
        public DashboardController(IMediator mediator, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mediator = mediator;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetOwnerDashboardQuery(CurrentUserId));
            if (!result.IsSuccess) return View("Error");
            return View(result.Value);
        }
    }
}
