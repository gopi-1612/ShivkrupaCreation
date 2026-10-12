using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Customer;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerListItemDto>> GetAllAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
        Task<CustomerDetailDto> GetByIdAsync(string id);
        Task SetActiveStatusAsync(string id, bool isActive);

        Task<CustomerDetailDto> GetProfileAsync(string userId);
        Task<CustomerDetailDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
    }
}
