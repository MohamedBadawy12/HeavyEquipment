using Microsoft.AspNetCore.Http;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadAsync(IFormFile file, CancellationToken ct = default);
        Task DeleteAsync(string url);
    }
}
