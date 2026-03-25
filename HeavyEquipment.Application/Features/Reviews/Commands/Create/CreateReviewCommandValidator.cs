using FluentValidation;

namespace HeavyEquipment.Application.Features.Reviews.Commands.Create
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("التقييم يجب أن يكون بين 1 و 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("التعليق مطلوب.")
                .MaximumLength(1000).WithMessage("التعليق طويل جداً.");

            RuleFor(x => x.RentalOrderId).NotEmpty();
            RuleFor(x => x.ReviewerId).NotEmpty();
        }
    }
}
