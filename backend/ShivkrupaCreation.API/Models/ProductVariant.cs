namespace ShivkrupaCreation.API.Models
{
    /// <summary>
    /// Represents a specific sellable combination of a product's color and size,
    /// each with its own SKU and stock so wholesale buyers can order by variant.
    /// </summary>
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Color { get; set; } = string.Empty;   // e.g. Maroon
        public string ColorHex { get; set; } = string.Empty; // e.g. #800000 for swatch UI
        public string Size { get; set; } = string.Empty;      // e.g. S, M, L, XL, Free Size

        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal? PriceOverride { get; set; } // null = use Product.WholesalePrice

        public bool IsActive { get; set; } = true;
    }
}
