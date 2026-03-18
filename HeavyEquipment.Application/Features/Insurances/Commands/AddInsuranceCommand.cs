using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Insurances.Commands
{
    public record AddInsuranceCommand(
        Guid RentalOrderId,
        decimal CoverageAmount,
        decimal PremiumAmount,
        DateTime ExpiryDate
    ) : IRequest<Result<Guid>>;

    public class AddInsuranceHandler : IRequestHandler<AddInsuranceCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddInsuranceHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(AddInsuranceCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId, cancellationToken);
            if (order is null)
                return Result<Guid>.Failure("الطلب غير موجود");

            if (order.Status != OrderStatus.Confirmed &&
                order.Status != OrderStatus.Active)
                return Result<Guid>.Failure("لا يمكن إضافة تأمين إلا بعد تأكيد الطلب");

            if (order.Insurance is not null)
                return Result<Guid>.Failure("يوجد تأمين مضاف مسبقاً لهذا الطلب");

            var policy = new InsurancePolicy(
                request.RentalOrderId,
                request.CoverageAmount,
                request.PremiumAmount,
                request.ExpiryDate);

            order.SetInsurance(policy);
            await _unitOfWork.InsurancePolicies.AddAsync(policy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(policy.Id);
        }
    }
}
