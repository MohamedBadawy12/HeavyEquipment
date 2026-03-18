using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Logistics.Commands
{
    public record UpdateLogisticsRateCommand(Guid Id, decimal NewRate) : IRequest<Result>;

    public class UpdateLogisticsRateHandler
        : IRequestHandler<UpdateLogisticsRateCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateLogisticsRateHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(
            UpdateLogisticsRateCommand request, CancellationToken cancellationToken)
        {
            var provider = await _unitOfWork.LogisticsProviders.GetByIdAsync(request.Id, cancellationToken);
            if (provider is null) return Result.Failure("شركة النقل غير موجودة");

            provider.UpdateRate(request.NewRate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
