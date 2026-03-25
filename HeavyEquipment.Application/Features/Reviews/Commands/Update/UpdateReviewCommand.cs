using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Reviews.Commands.Update
{
    public record UpdateReviewCommand(Guid ReviewId, string NewComment) : IRequest<Result>;

    public class UpdateReviewHandler : IRequestHandler<UpdateReviewCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateReviewHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);

            if (review == null)
                return Result.Failure("التقييم غير موجود.");

            try
            {
                review.EditComment(request.NewComment);

                _unitOfWork.Reviews.Update(review);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
