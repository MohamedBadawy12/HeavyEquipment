using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Inspections.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Inspections.Commands
{
    public record GetInspectionsByOrderQuery(Guid RentalOrderId)
        : IRequest<Result<IReadOnlyList<InspectionReportDto>>>;

    public class GetInspectionsByOrderHandler
        : IRequestHandler<GetInspectionsByOrderQuery, Result<IReadOnlyList<InspectionReportDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetInspectionsByOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<IReadOnlyList<InspectionReportDto>>> Handle(
            GetInspectionsByOrderQuery request, CancellationToken cancellationToken)
        {
            var reports = await _unitOfWork.InspectionReports
                .GetByRentalOrderIdAsync(request.RentalOrderId, cancellationToken);

            var dtos = reports.Select(r => new InspectionReportDto(
                r.Id, r.RentalOrderId, r.Type.ToString(),
                r.Result.ToString(), r.Notes, r.HoursReading,
                r.InspectionDate, r.InspectorId, r.PhotoUrls))
                .ToList();

            return Result<IReadOnlyList<InspectionReportDto>>.Success(dtos);
        }
    }
}
