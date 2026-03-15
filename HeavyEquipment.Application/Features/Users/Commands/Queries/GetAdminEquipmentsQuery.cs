using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Equipments.Commands.Queries;
using HeavyEquipment.Application.Features.Users.Dtos;
using HeavyEquipment.Domain.Enums;
using MediatR;

namespace HeavyEquipment.Application.Features.Users.Commands.Queries
{
    public record GetAdminEquipmentsQuery(
        string? Category = null,
        string? Status = null
    ) : IRequest<Result<AdminEquipmentsDto>>;

    public class GetAdminEquipmentsHandler
       : IRequestHandler<GetAdminEquipmentsQuery, Result<AdminEquipmentsDto>>
    {
        private readonly IMediator _mediator;

        public GetAdminEquipmentsHandler(IMediator mediator) => _mediator = mediator;

        public async Task<Result<AdminEquipmentsDto>> Handle(
            GetAdminEquipmentsQuery request, CancellationToken ct)
        {
            var pagedResult = await _mediator.Send(
                new GetAvailableEquipmentsQuery(DateTime.UtcNow, DateTime.UtcNow.AddYears(1),
                    PageSize: int.MaxValue), ct);
            var all = pagedResult?.Items?.ToList() ?? new();

            var filtered = all.AsEnumerable();
            if (!string.IsNullOrEmpty(request.Category))
                filtered = filtered.Where(e => e.Category == request.Category);
            if (!string.IsNullOrEmpty(request.Status))
                filtered = filtered.Where(e => e.Status == request.Status);

            var vm = new AdminEquipmentsDto
            {
                Equipments = filtered.ToList(),
                CategoryFilter = request.Category,
                StatusFilter = request.Status,
                TotalEquipments = all.Count,
                AvailableEquipments = all.Count(e => e.Status == EquipmentStatus.Available.ToString()),
                RentedEquipments = all.Count(e => e.Status == EquipmentStatus.Rented.ToString()),
            };

            return Result<AdminEquipmentsDto>.Success(vm);
        }
    }
}
