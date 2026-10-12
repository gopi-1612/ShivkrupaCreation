using Microsoft.Extensions.Options;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Services.Interfaces;
using ShivkrupaCreation.API.Settings;

namespace ShivkrupaCreation.API.Services
{
    /// <summary>
    /// Handles secure product image/video uploads: validates extension and size
    /// against a strict allow-list (never trusts the client-sent content type),
    /// generates a random file name (no user input reaches the filesystem path),
    /// and stores files under wwwroot/uploads so they're served as static files.
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        private readonly FileUploadSettings _settings;
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IOptions<FileUploadSettings> settings, IWebHostEnvironment env)
        {
            _settings = settings.Value;
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, UploadKind kind)
        {
            if (file is null || file.Length == 0)
            {
                throw ApiException.BadRequest("No file was uploaded.");
            }

            var allowedExtensions = kind == UploadKind.Image
                ? _settings.AllowedImageExtensions
                : _settings.AllowedVideoExtensions;

            var maxSizeBytes = (kind == UploadKind.Image ? _settings.MaxImageSizeMb : _settings.MaxVideoSizeMb) * 1024L * 1024L;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw ApiException.BadRequest(
                    $"Unsupported file type '{extension}'. Allowed types: {string.Join(", ", allowedExtensions)}");
            }

            if (file.Length > maxSizeBytes)
            {
                throw ApiException.BadRequest(
                    $"File is too large. Maximum allowed size is {(kind == UploadKind.Image ? _settings.MaxImageSizeMb : _settings.MaxVideoSizeMb)} MB.");
            }

            var relativeFolder = kind == UploadKind.Image
                ? "uploads/products/images"
                : "uploads/products/videos";

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var targetFolder = Path.Combine(webRoot, relativeFolder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(targetFolder);

            // Never trust the client's file name — generate our own to avoid path traversal / overwrite attacks.
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(targetFolder, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{relativeFolder}/{fileName}";
        }

        public void DeleteFile(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return;

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRoot, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
