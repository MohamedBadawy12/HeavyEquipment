using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Logistics.Commands
{
    public record AssignLogisticsCommand(
       Guid RentalOrderId,
       Guid LogisticsProviderId
    ) : IRequest<Result>;

    public class AssignLogisticsHandler : IRequestHandler<AssignLogisticsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignLogisticsHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(AssignLogisticsCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId, cancellationToken);
            if (order is null) return Result.Failure("الطلب غير موجود");

            var provider = await _unitOfWork.LogisticsProviders
                .GetByIdAsync(request.LogisticsProviderId, cancellationToken);
            if (provider is null) return Result.Failure("شركة النقل غير موجودة");
            if (!provider.IsActive) return Result.Failure("شركة النقل غير نشطة");

            order.AssignLogistics(provider);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
