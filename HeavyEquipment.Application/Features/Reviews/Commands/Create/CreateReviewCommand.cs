using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Reviews.Commands.Create
{
    public record CreateReviewCommand(
         Guid RentalOrderId,
         Guid ReviewerId,
         int Rating,
         string Comment,
         ReviewType Type) : IRequest<Result<Guid>>;

    public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateReviewHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Reviews.ExistsAsync(r =>
                r.RentalOrderId == request.RentalOrderId &&
                r.ReviewerId == request.ReviewerId &&
                r.Type == request.Type, cancellationToken);

            if (exists)
                return Result<Guid>.Failure("لقد قمت بإضافة تقييم لهذا الطلب مسبقاً.");

            var review = new Review(
                request.RentalOrderId,
                request.ReviewerId,
                request.Rating,
                request.Comment,
                request.Type
            );

            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result > 0)
                return Result<Guid>.Success(review.Id);

            return Result<Guid>.Failure("حدث خطأ أثناء حفظ التقييم.");
        }
    }
}
