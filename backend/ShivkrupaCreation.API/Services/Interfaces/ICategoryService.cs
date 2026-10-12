using ShivkrupaCreation.API.DTOs.Category;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync(bool includeInactive);
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryCreateDto dto);
        Task<CategoryDto> UpdateAsync(int id, CategoryUpdateDto dto);
        Task SetActiveStatusAsync(int id, bool isActive);
        Task DeleteAsync(int id);
    }
}
