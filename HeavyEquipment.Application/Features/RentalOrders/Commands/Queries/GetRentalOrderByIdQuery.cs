using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.RentalOrders.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Queries
{
    public record GetRentalOrderByIdQuery(Guid OrderId) : IRequest<Result<RentalOrderDto>>;
    public class GetRentalOrderByIdHandler : IRequestHandler<GetRentalOrderByIdQuery, Result<RentalOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetRentalOrderByIdHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<Result<RentalOrderDto>> Handle(GetRentalOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetWithDetailsAsync(request.OrderId, cancellationToken);
            if (order is null)
                return Result<RentalOrderDto>.Failure("الطلب غير موجود");

            var dto = new RentalOrderDto(
                order.Id,
                order.Equipment?.Name ?? "",
                order.Customer?.FullName ?? "",
                order.RentalStart,
                order.RentalEnd,
                order.TotalPrice,
                order.Status.ToString(),
                order.Insurance is not null,
                order.LogisticsProvider?.CompanyName,
                order.Inspections.Count,
                order.Reviews.Count);

            return Result<RentalOrderDto>.Success(dto);
        }
    }

}
