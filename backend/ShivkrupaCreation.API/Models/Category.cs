namespace ShivkrupaCreation.API.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Path/URL to category image shown on the Home page "Categories" section
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
