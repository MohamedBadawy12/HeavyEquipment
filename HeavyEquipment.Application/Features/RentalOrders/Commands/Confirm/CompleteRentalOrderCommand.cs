using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Confirm
{
    public record CompleteRentalOrderCommand(Guid OrderId, int HoursOperated) : IRequest<Result>;
    public class CompleteRentalOrderHandler : IRequestHandler<CompleteRentalOrderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompleteRentalOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(CompleteRentalOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetWithDetailsAsync(request.OrderId, cancellationToken);
            if (order is null) return Result.Failure("الطلب غير موجود");

            order.Complete();

            order.Equipment.LogOperatingHours(request.HoursOperated);
            order.Equipment.UpdateStatus(Domain.Enums.EquipmentStatus.Available);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
