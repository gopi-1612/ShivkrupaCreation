namespace ShivkrupaCreation.API.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public int ProductCount { get; set; }
    }

    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }

    public class CategoryUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
