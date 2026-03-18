using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Logistics.Commands
{
    public record ToggleLogisticsProviderCommand(Guid Id) : IRequest<Result>;
    public class ToggleLogisticsProviderHandler
        : IRequestHandler<ToggleLogisticsProviderCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ToggleLogisticsProviderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(
            ToggleLogisticsProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = await _unitOfWork.LogisticsProviders.GetByIdAsync(request.Id, cancellationToken);
            if (provider is null) return Result.Failure("شركة النقل غير موجودة");

            if (provider.IsActive) provider.Deactivate();
            else provider.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
