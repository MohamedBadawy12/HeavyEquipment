//using HeavyEquipment.Application.Common.Models;
//using HeavyEquipment.Application.Features.RentalOrders.Dtos;
//using HeavyEquipment.Domain.Interfaces;
//using MediatR;

//namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Queries
//{
//    public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<Result<IReadOnlyList<RentalOrderSummaryDto>>>;
//    public class GetCustomerOrdersHandler
//        : IRequestHandler<GetCustomerOrdersQuery, Result<IReadOnlyList<RentalOrderSummaryDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        public GetCustomerOrdersHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
//        public async Task<Result<IReadOnlyList<RentalOrderSummaryDto>>> Handle(
//            GetCustomerOrdersQuery request, CancellationToken cancellationToken)
//        {
//            var orders = await _unitOfWork.RentalOrders.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

//            var dtos = orders.Select(o => new RentalOrderSummaryDto(
//                o.Id,
//                o.EquipmentId,
//                o.Equipment?.Name ?? "",
//                o.RentalStart,
//                o.RentalEnd,
//                o.TotalPrice,
//                o.Status.ToString()))
//            .ToList();

//            return Result<IReadOnlyList<RentalOrderSummaryDto>>.Success(dtos);
//        }
//    }
//}
using HeavyEquipment.Application.Features.RentalOrders.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Queries
{
    public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<IReadOnlyList<RentalOrderSummaryDto>>;
    public class GetCustomerOrdersHandler
        : IRequestHandler<GetCustomerOrdersQuery, IReadOnlyList<RentalOrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCustomerOrdersHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<IReadOnlyList<RentalOrderSummaryDto>> Handle(
            GetCustomerOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.RentalOrders.
                GetByCustomerIdAsync(request.CustomerId, cancellationToken);

            var dtos = orders.Select(o => new RentalOrderSummaryDto(
                o.Id,
                o.EquipmentId,
                o.Equipment?.Name ?? "",
                o.RentalStart,
                o.RentalEnd,
                o.TotalPrice,
                o.Status.ToString()))
            .ToList();

            return dtos;
        }
    }
}

