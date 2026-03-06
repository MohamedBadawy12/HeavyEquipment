using FluentValidation;

namespace HeavyEquipment.Application.Features.RentalOrders.Commands.Create
{
    public class CreateRentalOrderValidator : AbstractValidator<CreateRentalOrderCommand>
    {
        public CreateRentalOrderValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.EquipmentId).NotEmpty();
            RuleFor(x => x.RentalStart).GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("تاريخ البدء لا يمكن أن يكون في الماضي");
            RuleFor(x => x.RentalEnd).GreaterThan(x => x.RentalStart)
                .WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء");
        }
    }
}
