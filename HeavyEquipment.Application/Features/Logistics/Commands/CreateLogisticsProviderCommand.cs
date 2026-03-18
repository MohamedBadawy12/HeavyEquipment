using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Logistics.Commands
{
    public record CreateLogisticsProviderCommand(
        string CompanyName,
        string ContactNumber,
        decimal RatePerKilometer
    ) : IRequest<Result<Guid>>;

    public class CreateLogisticsProviderHandler
        : IRequestHandler<CreateLogisticsProviderCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateLogisticsProviderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(
            CreateLogisticsProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = new LogisticsProvider(
                request.CompanyName,
                request.ContactNumber,
                request.RatePerKilometer);

            await _unitOfWork.LogisticsProviders.AddAsync(provider, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(provider.Id);
        }
    }
}
