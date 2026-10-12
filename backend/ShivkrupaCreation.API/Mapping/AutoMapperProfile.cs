using AutoMapper;
using ShivkrupaCreation.API.DTOs.Auth;
using ShivkrupaCreation.API.Models;

namespace ShivkrupaCreation.API.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // populated manually after role lookup

            // Category, Brand, and Product use straightforward manual mapping
            // inside their services (CategoryService, BrandService, ProductService)
            // since they combine multiple related entities (images, variants, counts).
        }
    }
}
