using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Data;
using ShivkrupaCreation.API.DTOs.Dashboard;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _db;

        public DashboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var customerRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Customer);
            var customerUserIds = customerRole is null
                ? new List<string>()
                : await _db.UserRoles.Where(ur => ur.RoleId == customerRole.Id).Select(ur => ur.UserId).ToListAsync();

            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var stats = new DashboardStatsDto
            {
                TotalProducts = await _db.Products.CountAsync(),
                ActiveProducts = await _db.Products.CountAsync(p => p.IsActive),
                InactiveProducts = await _db.Products.CountAsync(p => !p.IsActive),
                OutOfStockProducts = await _db.Products.CountAsync(p => p.StockQuantity <= 0),

                TotalCategories = await _db.Categories.CountAsync(),
                TotalBrands = await _db.Brands.CountAsync(),

                TotalCustomers = customerUserIds.Count,
                ActiveCustomers = await _db.Users.CountAsync(u => customerUserIds.Contains(u.Id) && u.IsActive),
                NewCustomersThisMonth = await _db.Users
                    .CountAsync(u => customerUserIds.Contains(u.Id) && u.CreatedAt >= startOfMonth),

                ProductsByCategory = await _db.Categories
                    .Select(c => new CategoryProductCountDto
                    {
                        CategoryName = c.Name,
                        ProductCount = c.Products.Count
                    })
                    .OrderByDescending(x => x.ProductCount)
                    .Take(10)
                    .ToListAsync(),

                RecentlyAddedProducts = await _db.Products
                    .Include(p => p.Images)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new RecentProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        PrimaryImageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
                        WholesalePrice = p.WholesalePrice,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync()
            };

            return stats;
        }
    }
}
