//using HeavyEquipment.Application.Common.Models;
//using HeavyEquipment.Application.Features.Reviews.Dtos;
//using HeavyEquipment.Domain.Interfaces;
//using MediatR;

//namespace HeavyEquipment.Application.Features.Reviews.Commands.Queries
//{
//    public record GetReviewsByEquipmentQuery(Guid EquipmentId)
//        : IRequest<Result<IReadOnlyList<ReviewDto>>>;

//    public class GetReviewsByEquipmentHandler
//        : IRequestHandler<GetReviewsByEquipmentQuery, Result<IReadOnlyList<ReviewDto>>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        public GetReviewsByEquipmentHandler(IUnitOfWork uow) => _unitOfWork = uow;

//        public async Task<Result<IReadOnlyList<ReviewDto>>> Handle(
//            GetReviewsByEquipmentQuery request, CancellationToken ct)
//        {
//            var reviews = await _unitOfWork.Reviews
//                .GetByEquipmentIdAsync(request.EquipmentId, ct);

//            var dtos = reviews.Select(r => new ReviewDto(
//                r.Id, r.RentalOrderId, r.ReviewerId,
//                r.Reviewer?.FullName ?? "",
//                r.Rating, r.Comment,
//                r.Type.ToString(), r.CreatedAt))
//                .ToList();

//            return Result<IReadOnlyList<ReviewDto>>.Success(dtos);
//        }
//    }
//}
