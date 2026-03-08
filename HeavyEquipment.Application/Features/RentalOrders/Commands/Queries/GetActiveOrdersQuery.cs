using HeavyEquipment.Application.Features.RentalOrders.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Queries
{
    public record GetActiveOrdersQuery()
        : IRequest<IReadOnlyList<RentalOrderSummaryDto>>;

    public class GetActiveOrdersHandler
        : IRequestHandler<GetActiveOrdersQuery, IReadOnlyList<RentalOrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveOrdersHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<RentalOrderSummaryDto>> Handle(
            GetActiveOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.RentalOrders.GetActiveOrdersAsync(cancellationToken);

            return orders.Select(o => new RentalOrderSummaryDto(
                o.Id,
                o.Equipment?.Name ?? "",
                o.RentalStart,
                o.RentalEnd,
                o.TotalPrice,
                o.Status.ToString()))
            .ToList();
        }
    }
}
