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
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId, cancellationToken);
            if (order is null)
                return Result<Guid>.Failure("الطلب غير موجود");

            if (order.Status != HeavyEquipment.Domain.Enums.OrderStatus.Completed)
                return Result<Guid>.Failure("يمكن التقييم فقط بعد اكتمال الطلب");

            var review = new Review(
                request.RentalOrderId,
                request.ReviewerId,
                request.Rating,
                request.Comment,
                request.Type);

            order.AddReview(review);
            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(review.Id);
        }
    }
}
