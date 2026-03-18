using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Inspections.Commands
{
    public record AddInspectionCommand(
        Guid RentalOrderId,
        Guid InspectorId,
        InspectionType Type,
        InspectionStatus Result,
        int HoursReading,
        string Notes = ""
    ) : IRequest<Result<Guid>>;

    public class AddInspectionHandler : IRequestHandler<AddInspectionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddInspectionHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(AddInspectionCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId, cancellationToken);
            if (order is null)
                return Result<Guid>.Failure("الطلب غير موجود");

            var report = new InspectionReport(
                request.RentalOrderId,
                request.InspectorId,
                request.Type,
                request.Result,
                request.HoursReading,
                request.Notes);

            order.AddInspection(report);
            await _unitOfWork.InspectionReports.AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(report.Id);
        }
    }
}
