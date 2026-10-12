namespace ShivkrupaCreation.API.DTOs.Product
{
    // ---------- Read ----------

    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Mrp { get; set; }
        public decimal WholesalePrice { get; set; }
        public int MinWholesaleQuantity { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public double AverageRating { get; set; }
        public bool InStock { get; set; }
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ProductVideoDto
    {
        public int Id { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string Color { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal? PriceOverride { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string Fabric { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string SetType { get; set; } = string.Empty;
        public string Work { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string BottomStyle { get; set; } = string.Empty;
        public string StitchType { get; set; } = string.Empty;
        public string SleeveStyle { get; set; } = string.Empty;
        public string Neck { get; set; } = string.Empty;
        public string Occasion { get; set; } = string.Empty;
        public decimal Mrp { get; set; }
        public decimal WholesalePrice { get; set; }
        public int MinWholesaleQuantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public double AverageRating { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;

        public List<ProductImageDto> Images { get; set; } = new();
        public List<ProductVideoDto> Videos { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();

        public List<string> AvailableColors => Variants.Select(v => v.Color).Distinct().ToList();
        public List<string> AvailableSizes => Variants.Select(v => v.Size).Distinct().ToList();
    }

    // ---------- Query (search / filter / sort / pagination) ----------

    public enum ProductSortBy
    {
        Newest,
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc
    }

    public class ProductQueryDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsFeatured { get; set; }

        // Admin listings may want to see inactive products too; public storefront always forces true.
        public bool? IsActive { get; set; } = true;

        public ProductSortBy SortBy { get; set; } = ProductSortBy.Newest;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    // ---------- Write ----------

    public class ProductVariantWriteDto
    {
        public int? Id { get; set; } // present when updating an existing variant
        public string Color { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal? PriceOverride { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string Fabric { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string SetType { get; set; } = string.Empty;
        public string Work { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string BottomStyle { get; set; } = string.Empty;
        public string StitchType { get; set; } = string.Empty;
        public string SleeveStyle { get; set; } = string.Empty;
        public string Neck { get; set; } = string.Empty;
        public string Occasion { get; set; } = string.Empty;
        public decimal Mrp { get; set; }
        public decimal WholesalePrice { get; set; }
        public int MinWholesaleQuantity { get; set; } = 1;
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public bool IsFeatured { get; set; } = false;
        public List<ProductVariantWriteDto> Variants { get; set; } = new();
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public bool IsActive { get; set; } = true;
    }
}
