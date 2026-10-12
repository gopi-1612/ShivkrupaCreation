namespace ShivkrupaCreation.API.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty; // base/master SKU

        public string? ShortDescription { get; set; }
        public string? Description { get; set; }

        public string Fabric { get; set; } = string.Empty;   // e.g. Cotton, Rayon
        public string Pattern { get; set; } = string.Empty;  // e.g. Printed, Solid
        public string SetType { get; set; } = string.Empty;
        public string Work { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string BottomStyle { get; set; } = string.Empty;
        public string StitchType { get; set; } = string.Empty;
        public string SleeveStyle { get; set; } = string.Empty;
        public string Neck { get; set; } = string.Empty;
        public string Occasion { get; set; } = string.Empty;

        // Wholesale pricing
        public decimal Mrp { get; set; }                 // reference retail price
        public decimal WholesalePrice { get; set; }       // price per unit at wholesale
        public int MinWholesaleQuantity { get; set; } = 1; // MOQ per order
        public int StockQuantity { get; set; }             // total available stock

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        public double AverageRating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVideo> Videos { get; set; } = new List<ProductVideo>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
