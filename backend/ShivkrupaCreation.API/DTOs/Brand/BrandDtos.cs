namespace ShivkrupaCreation.API.DTOs.Brand
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }

    public class BrandCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
    }

    public class BrandUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
