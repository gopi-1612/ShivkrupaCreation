namespace ShivkrupaCreation.API.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }

        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }

        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }

        public List<CategoryProductCountDto> ProductsByCategory { get; set; } = new();
        public List<RecentProductDto> RecentlyAddedProducts { get; set; } = new();
    }

    public class CategoryProductCountDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    public class RecentProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public decimal WholesalePrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
