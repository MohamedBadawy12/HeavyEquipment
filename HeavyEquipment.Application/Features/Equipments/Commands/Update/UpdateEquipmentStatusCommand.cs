using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Update
{
    public record UpdateEquipmentStatusCommand(Guid EquipmentId,
        EquipmentStatus NewStatus) : IRequest<Result>;

    public class UpdateEquipmentStatusHandler : IRequestHandler<UpdateEquipmentStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEquipmentStatusHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(UpdateEquipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
            if (equipment is null)
                return Result.Failure("المعدة غير موجودة");

            equipment.UpdateStatus(request.NewStatus);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}