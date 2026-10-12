using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Data;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Product;
using ShivkrupaCreation.API.Helpers;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileUploadService _fileUploadService;

        public ProductService(ApplicationDbContext db, IFileUploadService fileUploadService)
        {
            _db = db;
            _fileUploadService = fileUploadService;
        }

        // ---------- Read: listing with search/filter/sort/pagination ----------

        public async Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductQueryDto query)
        {
            var products = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .AsQueryable();

            if (query.IsActive.HasValue)
            {
                products = products.Where(p => p.IsActive == query.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim().ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Sku.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term));
            }

            if (query.CategoryId.HasValue)
                products = products.Where(p => p.CategoryId == query.CategoryId.Value);

            if (query.BrandId.HasValue)
                products = products.Where(p => p.BrandId == query.BrandId.Value);

            if (query.IsFeatured.HasValue)
                products = products.Where(p => p.IsFeatured == query.IsFeatured.Value);

            if (query.MinPrice.HasValue)
                products = products.Where(p => p.WholesalePrice >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                products = products.Where(p => p.WholesalePrice <= query.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(query.Color))
                products = products.Where(p => p.Variants.Any(v => v.Color == query.Color));

            if (!string.IsNullOrWhiteSpace(query.Size))
                products = products.Where(p => p.Variants.Any(v => v.Size == query.Size));

            products = query.SortBy switch
            {
                ProductSortBy.NameAsc => products.OrderBy(p => p.Name),
                ProductSortBy.NameDesc => products.OrderByDescending(p => p.Name),
                ProductSortBy.PriceAsc => products.OrderBy(p => p.WholesalePrice),
                ProductSortBy.PriceDesc => products.OrderByDescending(p => p.WholesalePrice),
                _ => products.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await products.CountAsync();

            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize is < 1 or > 100 ? 12 : query.PageSize;

            var items = await products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => MapToListItem(p))
                .ToListAsync();

            return new PagedResult<ProductListItemDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDetailDto> GetBySlugAsync(string slug)
        {
            var product = await FullProductQuery()
                .FirstOrDefaultAsync(p => p.Slug == slug)
                ?? throw ApiException.NotFound("Product not found.");

            return MapToDetail(product);
        }

        public async Task<ProductDetailDto> GetByIdAsync(int id)
        {
            var product = await FullProductQuery()
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw ApiException.NotFound("Product not found.");

            return MapToDetail(product);
        }

        // ---------- Write: CRUD ----------

        public async Task<ProductDetailDto> CreateAsync(ProductCreateDto dto)
        {
            await EnsureCategoryAndBrandExistAsync(dto.CategoryId, dto.BrandId);

            if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku))
            {
                throw ApiException.Conflict("A product with this SKU already exists.");
            }

            var slug = await GenerateUniqueSlugAsync(dto.Name);

            var product = new Product
            {
                Name = dto.Name,
                Slug = slug,
                Sku = dto.Sku,
                ShortDescription = dto.ShortDescription,
                Description = dto.Description,
                Fabric = dto.Fabric,
                Pattern = dto.Pattern,
                SetType = dto.SetType,
                Work = dto.Work,
                Style = dto.Style,
                BottomStyle = dto.BottomStyle,
                StitchType = dto.StitchType,
                SleeveStyle = dto.SleeveStyle,
                Neck = dto.Neck,
                Occasion = dto.Occasion,
                Mrp = dto.Mrp,
                WholesalePrice = dto.WholesalePrice,
                MinWholesaleQuantity = dto.MinWholesaleQuantity,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                IsFeatured = dto.IsFeatured,
                IsActive = true
            };

            foreach (var v in dto.Variants)
            {
                product.Variants.Add(new ProductVariant
                {
                    Color = v.Color,
                    ColorHex = v.ColorHex,
                    Size = v.Size,
                    Sku = v.Sku,
                    StockQuantity = v.StockQuantity,
                    PriceOverride = v.PriceOverride,
                    IsActive = v.IsActive
                });
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<ProductDetailDto> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _db.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw ApiException.NotFound("Product not found.");

            await EnsureCategoryAndBrandExistAsync(dto.CategoryId, dto.BrandId);

            if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku && p.Id != id))
            {
                throw ApiException.Conflict("A product with this SKU already exists.");
            }

            if (dto.Name != product.Name)
            {
                product.Slug = await GenerateUniqueSlugAsync(dto.Name, excludeProductId: id);
            }

            product.Name = dto.Name;
            product.Sku = dto.Sku;
            product.ShortDescription = dto.ShortDescription;
            product.Description = dto.Description;
            product.Fabric = dto.Fabric;
            product.Pattern = dto.Pattern;
            product.SetType = dto.SetType;
            product.Work = dto.Work;
            product.Style = dto.Style;
            product.BottomStyle = dto.BottomStyle;
            product.StitchType = dto.StitchType;
            product.SleeveStyle = dto.SleeveStyle;
            product.Neck = dto.Neck;
            product.Occasion = dto.Occasion;
            product.Mrp = dto.Mrp;
            product.WholesalePrice = dto.WholesalePrice;
            product.MinWholesaleQuantity = dto.MinWholesaleQuantity;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;
            product.IsFeatured = dto.IsFeatured;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            // Sync variants: update existing (by Id), add new (no Id), remove ones no longer present.
            var incomingIds = dto.Variants.Where(v => v.Id.HasValue).Select(v => v.Id!.Value).ToHashSet();
            var toRemove = product.Variants.Where(v => !incomingIds.Contains(v.Id)).ToList();
            _db.ProductVariants.RemoveRange(toRemove);

            foreach (var v in dto.Variants)
            {
                if (v.Id.HasValue)
                {
                    var existing = product.Variants.FirstOrDefault(pv => pv.Id == v.Id.Value);
                    if (existing is not null)
                    {
                        existing.Color = v.Color;
                        existing.ColorHex = v.ColorHex;
                        existing.Size = v.Size;
                        existing.Sku = v.Sku;
                        existing.StockQuantity = v.StockQuantity;
                        existing.PriceOverride = v.PriceOverride;
                        existing.IsActive = v.IsActive;
                    }
                }
                else
                {
                    product.Variants.Add(new ProductVariant
                    {
                        Color = v.Color,
                        ColorHex = v.ColorHex,
                        Size = v.Size,
                        Sku = v.Sku,
                        StockQuantity = v.StockQuantity,
                        PriceOverride = v.PriceOverride,
                        IsActive = v.IsActive
                    });
                }
            }

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task SetActiveStatusAsync(int id, bool isActive)
        {
            var product = await _db.Products.FindAsync(id) ?? throw ApiException.NotFound("Product not found.");
            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw ApiException.NotFound("Product not found.");

            foreach (var image in product.Images) _fileUploadService.DeleteFile(image.ImageUrl);
            foreach (var video in product.Videos) _fileUploadService.DeleteFile(video.VideoUrl);

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        // ---------- Image / video management ----------

        public async Task<ProductImageDto> AddImageAsync(int productId, IFormFile file, string? altText, bool isPrimary)
        {
            var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw ApiException.NotFound("Product not found.");

            var url = await _fileUploadService.SaveFileAsync(file, UploadKind.Image);

            if (isPrimary)
            {
                foreach (var existing in product.Images) existing.IsPrimary = false;
            }

            var image = new ProductImage
            {
                ProductId = productId,
                ImageUrl = url,
                AltText = string.IsNullOrWhiteSpace(altText) ? product.Name : altText,
                IsPrimary = isPrimary || !product.Images.Any(), // first image becomes primary automatically
                DisplayOrder = product.Images.Count
            };

            _db.ProductImages.Add(image);
            await _db.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText,
                IsPrimary = image.IsPrimary,
                DisplayOrder = image.DisplayOrder
            };
        }

        public async Task RemoveImageAsync(int productId, int imageId)
        {
            var image = await _db.ProductImages.FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId)
                ?? throw ApiException.NotFound("Image not found.");

            _fileUploadService.DeleteFile(image.ImageUrl);
            _db.ProductImages.Remove(image);
            await _db.SaveChangesAsync();

            // If the removed image was primary, promote the next available image.
            if (image.IsPrimary)
            {
                var next = await _db.ProductImages
                    .Where(i => i.ProductId == productId)
                    .OrderBy(i => i.DisplayOrder)
                    .FirstOrDefaultAsync();

                if (next is not null)
                {
                    next.IsPrimary = true;
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task<ProductVideoDto> AddVideoAsync(int productId, IFormFile file, string? title)
        {
            var product = await _db.Products.Include(p => p.Videos).FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw ApiException.NotFound("Product not found.");

            var url = await _fileUploadService.SaveFileAsync(file, UploadKind.Video);

            var video = new ProductVideo
            {
                ProductId = productId,
                VideoUrl = url,
                Title = string.IsNullOrWhiteSpace(title) ? product.Name : title,
                DisplayOrder = product.Videos.Count
            };

            _db.ProductVideos.Add(video);
            await _db.SaveChangesAsync();

            return new ProductVideoDto
            {
                Id = video.Id,
                VideoUrl = video.VideoUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                Title = video.Title,
                DisplayOrder = video.DisplayOrder
            };
        }

        public async Task RemoveVideoAsync(int productId, int videoId)
        {
            var video = await _db.ProductVideos.FirstOrDefaultAsync(v => v.Id == videoId && v.ProductId == productId)
                ?? throw ApiException.NotFound("Video not found.");

            _fileUploadService.DeleteFile(video.VideoUrl);
            _db.ProductVideos.Remove(video);
            await _db.SaveChangesAsync();
        }

        // ---------- Helpers ----------

        private IQueryable<Product> FullProductQuery() => _db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .Include(p => p.Videos.OrderBy(v => v.DisplayOrder))
            .Include(p => p.Variants);

        private async Task EnsureCategoryAndBrandExistAsync(int categoryId, int brandId)
        {
            if (!await _db.Categories.AnyAsync(c => c.Id == categoryId))
                throw ApiException.BadRequest("Selected category does not exist.");

            if (!await _db.Brands.AnyAsync(b => b.Id == brandId))
                throw ApiException.BadRequest("Selected brand does not exist.");
        }

        private async Task<string> GenerateUniqueSlugAsync(string name, int? excludeProductId = null)
        {
            var baseSlug = SlugHelper.GenerateSlug(name);
            var slug = baseSlug;
            var suffix = 1;

            while (await _db.Products.AnyAsync(p => p.Slug == slug && p.Id != excludeProductId))
            {
                slug = $"{baseSlug}-{suffix++}";
            }

            return slug;
        }

        private static ProductListItemDto MapToListItem(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Sku = p.Sku,
            Mrp = p.Mrp,
            WholesalePrice = p.WholesalePrice,
            MinWholesaleQuantity = p.MinWholesaleQuantity,
            PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                ?? p.Images.FirstOrDefault()?.ImageUrl,
            CategoryName = p.Category.Name,
            BrandName = p.Brand.Name,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            AverageRating = p.AverageRating,
            InStock = p.StockQuantity > 0 || p.Variants.Any(v => v.StockQuantity > 0)
        };

        private static ProductDetailDto MapToDetail(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Sku = p.Sku,
            ShortDescription = p.ShortDescription,
            Description = p.Description,
            Fabric = p.Fabric,
            Pattern = p.Pattern,
            SetType = p.SetType,
            Work = p.Work,
            Style = p.Style,
            BottomStyle = p.BottomStyle,
            StitchType = p.StitchType,
            SleeveStyle = p.SleeveStyle,
            Neck = p.Neck,
            Occasion = p.Occasion,
            Mrp = p.Mrp,
            WholesalePrice = p.WholesalePrice,
            MinWholesaleQuantity = p.MinWholesaleQuantity,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            AverageRating = p.AverageRating,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            BrandId = p.BrandId,
            BrandName = p.Brand.Name,
            Images = p.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                AltText = i.AltText,
                IsPrimary = i.IsPrimary,
                DisplayOrder = i.DisplayOrder
            }).ToList(),
            Videos = p.Videos.Select(v => new ProductVideoDto
            {
                Id = v.Id,
                VideoUrl = v.VideoUrl,
                ThumbnailUrl = v.ThumbnailUrl,
                Title = v.Title,
                DisplayOrder = v.DisplayOrder
            }).ToList(),
            Variants = p.Variants.Select(v => new ProductVariantDto
            {
                Id = v.Id,
                Color = v.Color,
                ColorHex = v.ColorHex,
                Size = v.Size,
                Sku = v.Sku,
                StockQuantity = v.StockQuantity,
                PriceOverride = v.PriceOverride,
                IsActive = v.IsActive
            }).ToList()
        };
    }
}
