namespace ShivkrupaCreation.API.Settings
{
    public class FileUploadSettings
    {
        public const string SectionName = "FileUploadSettings";

        public string ImagesPath { get; set; } = "wwwroot/uploads/products/images";
        public string VideosPath { get; set; } = "wwwroot/uploads/products/videos";
        public int MaxImageSizeMb { get; set; } = 5;
        public int MaxVideoSizeMb { get; set; } = 50;
        public string[] AllowedImageExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
        public string[] AllowedVideoExtensions { get; set; } = { ".mp4", ".webm", ".mov" };
    }
}
