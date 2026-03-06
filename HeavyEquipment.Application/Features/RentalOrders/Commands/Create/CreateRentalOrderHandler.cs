using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Create
{
    public class CreateRentalOrderHandler : IRequestHandler<CreateRentalOrderCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateRentalOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<Result<Guid>> Handle(CreateRentalOrderCommand request,
            CancellationToken cancellationToken)
        {
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
            if (equipment is null)
                return Result<Guid>.Failure("المعدة غير موجودة");

            if (!equipment.IsReadyForRental())
                return Result<Guid>.Failure("المعدة غير متاحة للتأجير حالياً");

            var isAvailable = await _unitOfWork.RentalOrders.IsEquipmentAvailableAsync(
                request.EquipmentId, request.RentalStart, request.RentalEnd, cancellationToken);

            if (!isAvailable)
                return Result<Guid>.Failure("المعدة محجوزة في هذه الفترة");

            var order = new RentalOrder(
                request.CustomerId,
                request.EquipmentId,
                request.RentalStart,
                request.RentalEnd,
                equipment.HourlyRate);

            equipment.AddRental(order);

            await _unitOfWork.RentalOrders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(order.Id);
        }
    }
}
