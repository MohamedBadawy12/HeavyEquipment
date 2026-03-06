using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Equipments.Dto;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Queries
{
    public record GetAvailableEquipmentsQuery(
        DateTime From,
        DateTime To,
        EquipmentCategory? Category = null,
        string? City = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<PaginatedResult<EquipmentSummaryDto>>;

    public class GetAvailableEquipmentsHandler
        : IRequestHandler<GetAvailableEquipmentsQuery, PaginatedResult<EquipmentSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAvailableEquipmentsHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PaginatedResult<EquipmentSummaryDto>> Handle(GetAvailableEquipmentsQuery request,
            CancellationToken cancellationToken)
        {
            var equipments = await _unitOfWork.Equipments
                 .GetAvailableEquipmentsAsync(request.From, request.To, cancellationToken);

            // Filter
            var filtered = equipments
                .Where(e => request.Category == null || e.Category == request.Category)
                .Where(e => string.IsNullOrEmpty(request.City) ||
                            e.CurrentLocation.City.Contains(request.City, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var totalCount = filtered.Count;

            var items = filtered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new EquipmentSummaryDto(
                    e.Id, e.Name,
                    e.Category.ToString(),
                    e.HourlyRate,
                    e.CurrentLocation.City,
                    e.Status.ToString()))
                .ToList();

            return new PaginatedResult<EquipmentSummaryDto>(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
