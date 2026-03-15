using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Queries;
using HeavyEquipment.Application.Features.RentalOrders.Dtos;
using HeavyEquipment.Application.Features.Users.Dtos;
using HeavyEquipment.Domain.Enums;
using MediatR;

namespace HeavyEquipment.Application.Features.Users.Commands.Queries
{
    public record GetAdminOrdersQuery(
        string? Status = null
    ) : IRequest<Result<AdminOrdersDto>>;
    public class GetAdminOrdersHandler
       : IRequestHandler<GetAdminOrdersQuery, Result<AdminOrdersDto>>
    {
        private readonly IMediator _mediator;

        public GetAdminOrdersHandler(IMediator mediator) => _mediator = mediator;

        public async Task<Result<AdminOrdersDto>> Handle(
            GetAdminOrdersQuery request, CancellationToken ct)
        {
            var all = ((await _mediator.Send(new GetActiveOrdersQuery(), ct))
                      ?? new List<RentalOrderSummaryDto>())
                      .ToList();

            var filtered = string.IsNullOrEmpty(request.Status)
                ? all
                : all.Where(o => o.Status == request.Status).ToList();

            var vm = new AdminOrdersDto
            {
                Orders = filtered,
                StatusFilter = request.Status,
                TotalOrders = all.Count,
                ActiveOrders = all.Count(o => o.Status == OrderStatus.Active.ToString()),
                PendingOrders = all.Count(o => o.Status == OrderStatus.Pending.ToString()),
                CompletedOrders = all.Count(o => o.Status == OrderStatus.Completed.ToString()),
                TotalRevenue = all
                    .Where(o => o.Status == OrderStatus.Completed.ToString())
                    .Sum(o => o.TotalPrice),
            };

            return Result<AdminOrdersDto>.Success(vm);
        }
    }
}
