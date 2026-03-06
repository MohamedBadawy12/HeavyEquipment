using HeavyEquipment.Application.Common.Models;
using MediatR;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Create
{
    public record CreateRentalOrderCommand(
       Guid CustomerId,
       Guid EquipmentId,
       DateTime RentalStart,
       DateTime RentalEnd
    ) : IRequest<Result<Guid>>;
}
