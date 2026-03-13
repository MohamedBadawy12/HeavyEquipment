using HeavyEquipment.Application.Features.Equipments.Commands.Queries;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Queries;
using HeavyEquipment.Domain.Enums;
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
        public DashboardController(IMediator mediator) => _mediator = mediator;

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var equipments = (await _mediator.Send(new GetEquipmentsByOwnerQuery(CurrentUserId)))?.ToList() ?? new();
            var allOrders = (await _mediator.Send(new GetCustomerOrdersQuery(CurrentUserId)))?.ToList() ?? new();
            var activeOrders = (await _mediator.Send(new GetActiveOrdersQuery()))?.ToList() ?? new();

            var ownerEquipmentIds = equipments.Select(e => e.Id).ToHashSet();
            var ownerActiveOrders = activeOrders
                .Where(o => ownerEquipmentIds.Contains(o.EquipmentId))
                .ToList();

            var now = DateTime.UtcNow;

            var vm = new DashboardViewModel
            {
                TotalEquipments = equipments.Count,
                AvailableEquipments = equipments.Count(e => e.Status == EquipmentStatus.Available.ToString()),
                RentedEquipments = equipments.Count(e => e.Status == EquipmentStatus.Rented.ToString()),
                UnderMaintenance = equipments.Count(e => e.Status == EquipmentStatus.UnderMaintenance.ToString()),

                ActiveRentals = ownerActiveOrders.Count,
                PendingRentals = allOrders.Count(o => o.Status == OrderStatus.Pending.ToString()),
                CompletedRentals = allOrders.Count(o => o.Status == OrderStatus.Completed.ToString()),

                TotalRevenue = allOrders
                    .Where(o => o.Status == OrderStatus.Completed.ToString())
                    .Sum(o => o.TotalPrice),

                MonthRevenue = allOrders
                    .Where(o => o.Status == OrderStatus.Completed.ToString()
                             && o.RentalEnd.Month == now.Month
                             && o.RentalEnd.Year == now.Year)
                    .Sum(o => o.TotalPrice),

                Equipments = equipments,
                RecentOrders = ownerActiveOrders
                    .OrderByDescending(o => o.RentalStart)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }
    }
}
