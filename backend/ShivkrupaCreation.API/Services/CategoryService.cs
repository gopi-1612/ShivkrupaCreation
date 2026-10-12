using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Data;
using ShivkrupaCreation.API.DTOs.Category;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.Helpers;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public CategoryService(ApplicationDbContext db, IMapper mapper, IWebHostEnvironment env)
        {
            _db = db;
            _mapper = mapper;
            _env = env;

        }

        public async Task<List<CategoryDto>> GetAllAsync(bool includeInactive)
        {
            var query = _db.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            var categories = await query.ToListAsync();
            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw ApiException.NotFound("Category not found.");

            return MapToDto(category);
        }

        public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
        {
            var slug = SlugHelper.GenerateSlug(dto.Name);

            if (await _db.Categories.AnyAsync(c => c.Slug == slug))
            {
                throw ApiException.Conflict("A category with a similar name already exists.");
            }


            string? imageUrl = null;

            if (dto.ImageUrl is not null && dto.ImageUrl.Length > 0)
            {
                var extension = Path.GetExtension(dto.ImageUrl.FileName)
                    .ToLowerInvariant();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                    throw ApiException.BadRequest("Unsupported Image format");

                var folder = Path.Combine(
                    _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
                    "uploads",
                    "categories");

                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ImageUrl.CopyToAsync(stream);

                imageUrl = $"/uploads/categories/{fileName}";
            }

            var category = new Category
            {
                Name = dto.Name,
                Slug = slug,
                Description = dto.Description,
                ImageUrl = imageUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _db.Categories.FindAsync(id)
                ?? throw ApiException.NotFound("Category not found.");

            var newSlug = SlugHelper.GenerateSlug(dto.Name);
            if (newSlug != category.Slug && await _db.Categories.AnyAsync(c => c.Slug == newSlug && c.Id != id))
            {
                throw ApiException.Conflict("A category with a similar name already exists.");
            }

            if (dto.ImageUrl is not null && dto.ImageUrl.Length > 0)
            {
                var extension = Path.GetExtension(dto.ImageUrl.FileName)
                    .ToLowerInvariant();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                    throw ApiException.BadRequest("Unsupported Image format");

                var folder = Path.Combine(
                    _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
                    "uploads",
                    "categories");

                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ImageUrl.CopyToAsync(stream);
                category.ImageUrl = $"/uploads/categories/{fileName}";
            }


            category.Name = dto.Name;
            category.Slug = newSlug;
            category.Description = dto.Description;

            category.DisplayOrder = dto.DisplayOrder;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return MapToDto(category);
        }


        public async Task SetActiveStatusAsync(int id, bool isActive)
        {
            var category = await _db.Categories.FindAsync(id)
                ?? throw ApiException.NotFound("Category not found.");

            category.IsActive = isActive;
            category.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw ApiException.NotFound("Category not found.");

            if (category.Products.Any())
            {
                throw ApiException.Conflict(
                    "This category has products linked to it. Deactivate it instead, or move/delete its products first.");
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }

        private static CategoryDto MapToDto(Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            IsActive = c.IsActive,
            DisplayOrder = c.DisplayOrder,
            ProductCount = c.Products?.Count ?? 0
        };
    }
}
