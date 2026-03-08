using HeavyEquipment.Application.Features.Equipments.Commands.Queries;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Queries;
using HeavyEquipment.WebMVC.ViewModels.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeavyEquipment.WebMVC.Controllers
{
    [Authorize(Roles = "Owner,Admin")]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<IActionResult> Index()
        {
            var equipments = await _mediator.Send(
                new GetEquipmentsByOwnerQuery(CurrentUserId));

            var activeOrders = await _mediator.Send(
                new GetActiveOrdersQuery());

            var vm = new DashboardViewModel
            {
                TotalEquipments = equipments?.Count() ?? 0,
                ActiveRentals = activeOrders?.Count() ?? 0,
                Equipments = equipments?.ToList() ?? new(),
                RecentOrders = activeOrders?.Take(5).ToList() ?? new()
            };

            return View(vm);
        }
    }
}
