using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Dashboard.Dtos;
using HeavyEquipment.Application.Features.Equipments.Commands.Queries;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Queries;
using HeavyEquipment.Domain.Enums;
using MediatR;

namespace HeavyEquipment.Application.Features.Dashboard.Commands
{
    public record GetOwnerDashboardQuery(Guid OwnerId)
        : IRequest<Result<OwnerDashboardDto>>;

    public class GetOwnerDashboardHandler
        : IRequestHandler<GetOwnerDashboardQuery, Result<OwnerDashboardDto>>
    {
        private readonly IMediator _mediator;

        public GetOwnerDashboardHandler(IMediator mediator)
            => _mediator = mediator;

        public async Task<Result<OwnerDashboardDto>> Handle(
            GetOwnerDashboardQuery request, CancellationToken cancellationToken)
        {
            var equipments = (await _mediator.Send(
                new GetEquipmentsByOwnerQuery(request.OwnerId), cancellationToken))
                ?.ToList() ?? new();

            var allOrders = (await _mediator.Send(
                new GetOrdersByOwnerQuery(request.OwnerId), cancellationToken))
                ?.ToList() ?? new();

            var activeOrders = (await _mediator.Send(
                new GetActiveOrdersQuery(), cancellationToken))
                ?.ToList() ?? new();

            var ownerEquipmentIds = equipments.Select(e => e.Id).ToHashSet();

            var ownerActiveOrders = activeOrders
                .Where(o => ownerEquipmentIds.Contains(o.EquipmentId))
                .ToList();

            var now = DateTime.UtcNow;

            var dto = new OwnerDashboardDto
            {
                TotalEquipments = equipments.Count,
                AvailableEquipments = equipments.Count(e => e.Status == EquipmentStatus.Available.ToString()),
                RentedEquipments = equipments.Count(e => e.Status == EquipmentStatus.Rented.ToString()),
                UnderMaintenance = equipments.Count(e => e.Status == EquipmentStatus.UnderMaintenance.ToString()),

                ActiveRentals = ownerActiveOrders.Count(o => o.Status == OrderStatus.Active.ToString()),
                PendingRentals = ownerActiveOrders.Count(o => o.Status == OrderStatus.Pending.ToString()),
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
                    .Where(o => o.Status == OrderStatus.Active.ToString()
                             || o.Status == OrderStatus.Confirmed.ToString())
                    .OrderByDescending(o => o.RentalStart)
                    .Take(5)
                    .ToList(),

                PendingOrders = ownerActiveOrders
                    .Where(o => o.Status == OrderStatus.Pending.ToString())
                    .OrderBy(o => o.RentalStart)
                    .ToList()
            };

            return Result<OwnerDashboardDto>.Success(dto);
        }
    }
}
