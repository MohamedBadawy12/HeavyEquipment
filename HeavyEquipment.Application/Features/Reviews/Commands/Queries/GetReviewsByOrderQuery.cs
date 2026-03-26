using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Reviews.Dtos;
using HeavyEquipment.Domain.Interfaces;
using MediatR;

namespace HeavyEquipment.Application.Features.Reviews.Commands.Queries
{
    public record GetReviewsByOrderQuery(Guid RentalOrderId) : IRequest<Result<IReadOnlyList<ReviewDto>>>;

    public class GetReviewsByOrderHandler : IRequestHandler<GetReviewsByOrderQuery, Result<IReadOnlyList<ReviewDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetReviewsByOrderHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<Result<IReadOnlyList<ReviewDto>>> Handle
            (GetReviewsByOrderQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.Reviews
                .GetByRentalOrderIdAsync(request.RentalOrderId, cancellationToken);

            var dtos = reviews.Select(r => new ReviewDto(
                r.Id, r.RentalOrderId, r.ReviewerId,
                r.Reviewer?.FullName ?? "",
                r.Rating, r.Comment,
                r.Type.ToString(), r.CreatedAt))
                .ToList();

            return Result<IReadOnlyList<ReviewDto>>.Success(dtos);
        }
    }
}
