namespace ShivkrupaCreation.API.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty; // accessibility (alt attribute)

        public bool IsPrimary { get; set; } = false; // shown as default main preview
        public int DisplayOrder { get; set; } = 0;   // controls carousel/thumbnail order

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
