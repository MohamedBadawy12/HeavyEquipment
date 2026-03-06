using FluentValidation;

namespace HeavyEquipment.Application.Features.Equipments.Commands.Create
{
    public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
    {
        public CreateEquipmentCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
            RuleFor(x => x.HourlyRate).GreaterThan(0);
            RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ManufactureYear).InclusiveBetween(1900, DateTime.UtcNow.Year);
            RuleFor(x => x.MaintenanceThreshold).GreaterThan(0);
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}
