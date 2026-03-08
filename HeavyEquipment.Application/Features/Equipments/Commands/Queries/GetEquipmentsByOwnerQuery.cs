using HeavyEquipment.Application.Features.Equipments.Dto;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Queries
{
    public record GetEquipmentsByOwnerQuery(Guid OwnerId)
        : IRequest<IReadOnlyList<EquipmentSummaryDto>>;

    public class GetEquipmentsByOwnerHandler
       : IRequestHandler<GetEquipmentsByOwnerQuery, IReadOnlyList<EquipmentSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEquipmentsByOwnerHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<EquipmentSummaryDto>> Handle(
            GetEquipmentsByOwnerQuery request, CancellationToken cancellationToken)
        {
            var equipments = await _unitOfWork.Equipments
                .GetByOwnerIdAsync(request.OwnerId, cancellationToken);

            return equipments.Select(e => new EquipmentSummaryDto(
                e.Id,
                e.Name,
                e.Category.ToString(),
                e.HourlyRate,
                e.CurrentLocation.City,
                e.Status.ToString()))
            .ToList();
        }
    }
}
