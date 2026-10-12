using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Data;
using ShivkrupaCreation.API.DTOs.Brand;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Helpers;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Services
{
    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _db;

        public BrandService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<BrandDto>> GetAllAsync(bool includeInactive)
        {
            var query = _db.Brands.Include(b => b.Products).OrderBy(b => b.Name).AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(b => b.IsActive);
            }

            var brands = await query.ToListAsync();
            return brands.Select(MapToDto).ToList();
        }

        public async Task<BrandDto> GetByIdAsync(int id)
        {
            var brand = await _db.Brands.Include(b => b.Products).FirstOrDefaultAsync(b => b.Id == id)
                ?? throw ApiException.NotFound("Brand not found.");

            return MapToDto(brand);
        }

        public async Task<BrandDto> CreateAsync(BrandCreateDto dto)
        {
            var slug = SlugHelper.GenerateSlug(dto.Name);

            if (await _db.Brands.AnyAsync(b => b.Slug == slug))
            {
                throw ApiException.Conflict("A brand with a similar name already exists.");
            }

            var brand = new Brand
            {
                Name = dto.Name,
                Slug = slug,
                LogoUrl = dto.LogoUrl,
                Description = dto.Description,
                IsActive = true
            };

            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
            return MapToDto(brand);
        }

        public async Task<BrandDto> UpdateAsync(int id, BrandUpdateDto dto)
        {
            var brand = await _db.Brands.FindAsync(id) ?? throw ApiException.NotFound("Brand not found.");

            var newSlug = SlugHelper.GenerateSlug(dto.Name);
            if (newSlug != brand.Slug && await _db.Brands.AnyAsync(b => b.Slug == newSlug && b.Id != id))
            {
                throw ApiException.Conflict("A brand with a similar name already exists.");
            }

            brand.Name = dto.Name;
            brand.Slug = newSlug;
            brand.LogoUrl = dto.LogoUrl;
            brand.Description = dto.Description;
            brand.IsActive = dto.IsActive;
            brand.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return MapToDto(brand);
        }

        public async Task SetActiveStatusAsync(int id, bool isActive)
        {
            var brand = await _db.Brands.FindAsync(id) ?? throw ApiException.NotFound("Brand not found.");
            brand.IsActive = isActive;
            brand.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _db.Brands.Include(b => b.Products).FirstOrDefaultAsync(b => b.Id == id)
                ?? throw ApiException.NotFound("Brand not found.");

            if (brand.Products.Any())
            {
                throw ApiException.Conflict(
                    "This brand has products linked to it. Deactivate it instead, or move/delete its products first.");
            }

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();
        }

        private static BrandDto MapToDto(Brand b) => new()
        {
            Id = b.Id,
            Name = b.Name,
            Slug = b.Slug,
            LogoUrl = b.LogoUrl,
            Description = b.Description,
            IsActive = b.IsActive,
            ProductCount = b.Products?.Count ?? 0
        };
    }
}
