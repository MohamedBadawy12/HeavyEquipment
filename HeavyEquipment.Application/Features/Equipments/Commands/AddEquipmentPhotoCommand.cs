using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HeavyEquipment.Application.Features.Equipments.Commands
{
    public record AddEquipmentPhotoCommand(
        Guid EquipmentId,
        Guid OwnerId,
        List<IFormFile> Photos
    ) : IRequest<Result<List<string>>>;

    public class AddEquipmentPhotoHandler : IRequestHandler<AddEquipmentPhotoCommand, Result<List<string>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public AddEquipmentPhotoHandler(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<Result<List<string>>> Handle(
            AddEquipmentPhotoCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
            if (equipment is null) return Result<List<string>>.Failure("المعدة غير موجودة");
            if (equipment.OwnerId != request.OwnerId) return Result<List<string>>.Failure("غير مصرح لك");

            var remaining = 5 - equipment.PhotoUrls.Count;
            if (remaining <= 0) return Result<List<string>>.Failure("لا يمكن إضافة أكثر من 5 صور");

            var photosToUpload = request.Photos.Take(remaining).ToList();
            var uploadedUrls = new List<string>();

            foreach (var photo in photosToUpload)
            {
                var url = await _imageService.UploadAsync(photo, cancellationToken);
                equipment.AddPhoto(url);
                uploadedUrls.Add(url);
            }

            _unitOfWork.Equipments.Update(equipment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<List<string>>.Success(uploadedUrls);
        }
    }

}
