using ShivkrupaCreation.API.DTOs.Brand;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllAsync(bool includeInactive);
        Task<BrandDto> GetByIdAsync(int id);
        Task<BrandDto> CreateAsync(BrandCreateDto dto);
        Task<BrandDto> UpdateAsync(int id, BrandUpdateDto dto);
        Task SetActiveStatusAsync(int id, bool isActive);
        Task DeleteAsync(int id);
    }
}
