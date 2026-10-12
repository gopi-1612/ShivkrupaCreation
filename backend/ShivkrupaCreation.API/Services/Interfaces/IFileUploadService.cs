namespace ShivkrupaCreation.API.Services.Interfaces
{
    public enum UploadKind { Image, Video }

    public interface IFileUploadService
    {
        /// <summary>Validates and saves a file, returning its public relative URL (e.g. /uploads/products/images/xyz.jpg).</summary>
        Task<string> SaveFileAsync(IFormFile file, UploadKind kind);

        /// <summary>Deletes a previously uploaded file given its stored relative URL. Safe no-op if the file is missing.</summary>
        void DeleteFile(string relativeUrl);
    }
}
