using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Domain.ValueObjects;
using MediatR;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Create
{
    public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEquipmentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<Result<Guid>> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var location = new Location(request.City, request.Address, request.Latitude, request.Longitude);

            var equipment = new Equipment(
                request.Name, request.Description, request.Model,
                request.ManufactureYear, request.Category,
                request.HourlyRate, request.DepositAmount,
                request.MaintenanceThreshold, request.OwnerId, location);

            await _unitOfWork.Equipments.AddAsync(equipment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(equipment.Id);
        }
    }
}
