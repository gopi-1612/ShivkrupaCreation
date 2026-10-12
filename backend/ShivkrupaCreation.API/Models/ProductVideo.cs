namespace ShivkrupaCreation.API.Models
{
    public class ProductVideo
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string Title { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
