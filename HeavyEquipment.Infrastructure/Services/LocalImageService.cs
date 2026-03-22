using HeavyEquipment.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HeavyEquipment.Infrastructure.Services
{
    public class LocalImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LocalImageService> _logger;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        public LocalImageService(IWebHostEnvironment env, ILogger<LocalImageService> logger)
        {
            _env = env;
            _logger = logger;
        }


        public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0)
                throw new InvalidOperationException("الملف فارغ");

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("حجم الصورة يجب أن يكون أقل من 5MB");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new InvalidOperationException("نوع الملف غير مدعوم. استخدم JPG, PNG, أو WEBP");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "equipments");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            _logger.LogInformation("Image uploaded: {FileName}", fileName);

            return $"/uploads/equipments/{fileName}";
        }
        public Task DeleteAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) return Task.CompletedTask;

            var filePath = Path.Combine(_env.WebRootPath, url.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Image deleted: {Url}", url);
            }

            return Task.CompletedTask;
        }
    }
}
