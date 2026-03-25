using FluentValidation;

namespace HeavyEquipment.Application.Features.Reviews.Commands.Update
{
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();

            RuleFor(x => x.NewComment)
                .NotEmpty().WithMessage("التعليق لا يمكن أن يكون فارغاً.")
                .MaximumLength(1000).WithMessage("التعليق طويل جداً.");
        }
    }
}
