using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Enums;
using MediatR;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Create
{
    public record CreateEquipmentCommand(
        string Name,
        string Description,
        string Model,
        int ManufactureYear,
        EquipmentCategory Category,
        decimal HourlyRate,
        decimal DepositAmount,
        int MaintenanceThreshold,
        Guid OwnerId,
        string City,
        string Address,
        double Latitude,
        double Longitude
    ) : IRequest<Result<Guid>>;
}
