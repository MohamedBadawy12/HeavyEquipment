using FluentValidation;
using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Cancel
{
    public record CancelRentalOrderCommand(Guid OrderId, string Reason) : IRequest<Result>;
    public class CancelRentalOrderValidator : AbstractValidator<CancelRentalOrderCommand>
    {
        public CancelRentalOrderValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500)
                .WithMessage("سبب الإلغاء مطلوب");
        }
    }
    public class CancelRentalOrderHandler : IRequestHandler<CancelRentalOrderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancelRentalOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(CancelRentalOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null) return Result.Failure("الطلب غير موجود");

            order.Cancel(request.Reason);

            var equipment = await _unitOfWork.Equipments.GetByIdAsync(order.EquipmentId, cancellationToken);
            equipment?.UpdateStatus(Domain.Enums.EquipmentStatus.Available);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
