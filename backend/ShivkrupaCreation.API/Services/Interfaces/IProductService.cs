using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Product;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductQueryDto query);
        Task<ProductDetailDto> GetBySlugAsync(string slug);
        Task<ProductDetailDto> GetByIdAsync(int id);

        Task<ProductDetailDto> CreateAsync(ProductCreateDto dto);
        Task<ProductDetailDto> UpdateAsync(int id, ProductUpdateDto dto);
        Task SetActiveStatusAsync(int id, bool isActive);
        Task DeleteAsync(int id);

        Task<ProductImageDto> AddImageAsync(int productId, IFormFile file, string? altText, bool isPrimary);
        Task RemoveImageAsync(int productId, int imageId);

        Task<ProductVideoDto> AddVideoAsync(int productId, IFormFile file, string? title);
        Task RemoveVideoAsync(int productId, int videoId);
    }
}
