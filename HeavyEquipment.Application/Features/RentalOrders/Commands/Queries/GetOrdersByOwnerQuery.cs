using HeavyEquipment.Application.Features.RentalOrders.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Queries
{
    public record GetOrdersByOwnerQuery(Guid OwnerId)
    : IRequest<IReadOnlyList<RentalOrderSummaryDto>>;

    public class GetOrdersByOwnerHandler
    : IRequestHandler<GetOrdersByOwnerQuery, IReadOnlyList<RentalOrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetOrdersByOwnerHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<RentalOrderSummaryDto>> Handle(
            GetOrdersByOwnerQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.RentalOrders
                .GetByEquipmentOwnerIdAsync(request.OwnerId, cancellationToken);

            return orders.Select(o => new RentalOrderSummaryDto(
                o.Id,
                o.EquipmentId,
                o.Equipment?.Name ?? "",
                o.RentalStart,
                o.RentalEnd,
                o.TotalPrice,
                o.Status.ToString()))
                .ToList();
        }
    }
}
