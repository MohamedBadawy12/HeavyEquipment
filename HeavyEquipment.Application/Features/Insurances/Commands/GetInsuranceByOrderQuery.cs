using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Insurances.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Insurances.Commands
{
    public record GetInsuranceByOrderQuery(Guid RentalOrderId)
        : IRequest<Result<InsurancePolicyDto>>;

    public class GetInsuranceByOrderHandler
       : IRequestHandler<GetInsuranceByOrderQuery, Result<InsurancePolicyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetInsuranceByOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<InsurancePolicyDto>> Handle(
            GetInsuranceByOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId, cancellationToken);
            if (order?.Insurance is null)
                return Result<InsurancePolicyDto>.Failure("لا يوجد تأمين لهذا الطلب");

            var p = order.Insurance;
            return Result<InsurancePolicyDto>.Success(new InsurancePolicyDto(
                p.Id, p.RentalOrderId, p.PolicyNumber,
                p.CoverageAmount, p.PremiumAmount, p.ExpiryDate,
                p.IsClaimed, p.ClaimReason, p.ClaimedAt));
        }
    }
}
