using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Confirm
{
    public record MarkOrderAsActiveCommand(Guid OrderId) : IRequest<Result>;
    public class MarkOrderAsActiveHandler : IRequestHandler<MarkOrderAsActiveCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MarkOrderAsActiveHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(MarkOrderAsActiveCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null) return Result.Failure("الطلب غير موجود");

            order.MarkAsActive();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
