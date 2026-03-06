using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Confirm
{
    public record ConfirmRentalOrderCommand(Guid OrderId) : IRequest<Result>;
    public class ConfirmRentalOrderHandler : IRequestHandler<ConfirmRentalOrderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmRentalOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(ConfirmRentalOrderCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null) return Result.Failure("الطلب غير موجود");

            order.Confirm();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
