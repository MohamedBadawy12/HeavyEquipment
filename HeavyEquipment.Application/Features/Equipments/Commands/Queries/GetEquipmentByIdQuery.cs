using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Equipments.Dto;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Queries
{
    public record GetEquipmentByIdQuery(Guid EquipmentId) : IRequest<Result<EquipmentDto>>;

    public class GetEquipmentByIdHandler : IRequestHandler<GetEquipmentByIdQuery, Result<EquipmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetEquipmentByIdHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<EquipmentDto>> Handle(GetEquipmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var e = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
            if (e is null)
                return Result<EquipmentDto>.Failure("المعدة غير موجودة");

            var dto = new EquipmentDto(
                e.Id, e.Name, e.Description, e.Model,
                e.ManufactureYear, e.Category.ToString(),
                e.Status.ToString(), e.HourlyRate, e.DepositAmount,
                e.TotalHoursOperated, e.NextMaintenanceThreshold, e.PhotoUrls.ToList(),
                e.CurrentLocation.City, e.CurrentLocation.Address,
                e.CurrentLocation.Latitude, e.CurrentLocation.Longitude, e.OwnerId,
                e.Owner?.FullName ?? "", e.Owner?.TrustScore ?? 0, e.Owner?.IsVerified ?? false);

            return Result<EquipmentDto>.Success(dto);
        }
    }
}
