using FluentValidation;
using ShivkrupaCreation.API.DTOs.Category;
using ShivkrupaCreation.API.DTOs.Brand;

namespace ShivkrupaCreation.API.Validators
{
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(1000);
        }
    }

    public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(1000);
        }
    }

    public class BrandCreateDtoValidator : AbstractValidator<BrandCreateDto>
    {
        public BrandCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }

    public class BrandUpdateDtoValidator : AbstractValidator<BrandUpdateDto>
    {
        public BrandUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }
}
