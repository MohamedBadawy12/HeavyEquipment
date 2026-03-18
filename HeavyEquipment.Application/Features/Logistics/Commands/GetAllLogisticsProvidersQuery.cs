using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Logistics.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Logistics.Commands
{
    public record GetAllLogisticsProvidersQuery()
        : IRequest<Result<IReadOnlyList<LogisticsProviderAdminDto>>>;

    public class GetAllLogisticsProvidersHandler
       : IRequestHandler<GetAllLogisticsProvidersQuery, Result<IReadOnlyList<LogisticsProviderAdminDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllLogisticsProvidersHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<IReadOnlyList<LogisticsProviderAdminDto>>> Handle(
            GetAllLogisticsProvidersQuery request, CancellationToken cancellationToken)
        {
            var providers = await _unitOfWork.LogisticsProviders.GetAllAsync(cancellationToken);
            var dtos = providers
                .OrderByDescending(p => p.IsActive)
                .ThenBy(p => p.CompanyName)
                .Select(p => new LogisticsProviderAdminDto(
                    p.Id, p.CompanyName, p.ContactNumber,
                    p.RatePerKilometer, p.IsActive))
                .ToList();
            return Result<IReadOnlyList<LogisticsProviderAdminDto>>.Success(dtos);
        }
    }
}
