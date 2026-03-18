using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Logistics.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Logistics.Commands
{
    public record GetLogisticsProvidersQuery() : IRequest<Result<IReadOnlyList<LogisticsProviderDto>>>;

    public class GetLogisticsProvidersHandler
       : IRequestHandler<GetLogisticsProvidersQuery, Result<IReadOnlyList<LogisticsProviderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetLogisticsProvidersHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<IReadOnlyList<LogisticsProviderDto>>> Handle(
            GetLogisticsProvidersQuery request, CancellationToken cancellationToken)
        {
            var providers = await _unitOfWork.LogisticsProviders.GetAllAsync(cancellationToken);
            var dtos = providers
                .Where(p => p.IsActive)
                .Select(p => new LogisticsProviderDto(
                    p.Id, p.CompanyName, p.ContactNumber,
                    p.RatePerKilometer, p.IsActive))
                .ToList();

            return Result<IReadOnlyList<LogisticsProviderDto>>.Success(dtos);
        }
    }
}
