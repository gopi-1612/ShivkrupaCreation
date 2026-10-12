using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShivkrupaCreation.API.Data;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Customer;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<PagedResult<CustomerListItemDto>> GetAllAsync(
            string? searchTerm, bool? isActive, int pageNumber, int pageSize)
        {
            // Only customers (not admins) show up in customer management.
            var customerRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Customer);
            var customerUserIds = customerRole is null
                ? Enumerable.Empty<string>()
                : await _db.UserRoles.Where(ur => ur.RoleId == customerRole.Id).Select(ur => ur.UserId).ToListAsync();

            var query = _db.Users.Where(u => customerUserIds.Contains(u.Id)).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(term) ||
                    u.Email!.ToLower().Contains(term) ||
                    (u.BusinessName != null && u.BusinessName.ToLower().Contains(term)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CustomerListItemDto>
            {
                Items = users.Select(MapToListItem).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CustomerDetailDto> GetByIdAsync(string id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id)
                ?? throw ApiException.NotFound("Customer not found.");

            return MapToDetail(user);
        }

        public async Task SetActiveStatusAsync(string id, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id) ?? throw ApiException.NotFound("Customer not found.");
            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        public async Task<CustomerDetailDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw ApiException.NotFound("User not found.");
            return MapToDetail(user);
        }

        public async Task<CustomerDetailDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw ApiException.NotFound("User not found.");

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.BusinessName = dto.BusinessName;
            user.GstNumber = dto.GstNumber;
            user.Address = dto.Address;
            user.City = dto.City;
            user.State = dto.State;
            user.PinCode = dto.PinCode;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw ApiException.BadRequest("Could not update profile.", result.Errors.Select(e => e.Description).ToList());
            }

            return MapToDetail(user);
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                throw ApiException.BadRequest("New password and confirmation do not match.");
            }

            var user = await _userManager.FindByIdAsync(userId) ?? throw ApiException.NotFound("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                throw ApiException.BadRequest("Could not change password.", result.Errors.Select(e => e.Description).ToList());
            }
        }

        private static CustomerListItemDto MapToListItem(ApplicationUser u) => new()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email ?? string.Empty,
            PhoneNumber = u.PhoneNumber,
            BusinessName = u.BusinessName,
            City = u.City,
            State = u.State,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };

        private static CustomerDetailDto MapToDetail(ApplicationUser u) => new()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email ?? string.Empty,
            PhoneNumber = u.PhoneNumber,
            BusinessName = u.BusinessName,
            City = u.City,
            State = u.State,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            GstNumber = u.GstNumber,
            Address = u.Address,
            PinCode = u.PinCode
        };
    }
}
