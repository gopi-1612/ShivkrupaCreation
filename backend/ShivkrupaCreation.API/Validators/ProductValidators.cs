using FluentValidation;
using ShivkrupaCreation.API.DTOs.Product;

namespace ShivkrupaCreation.API.Validators
{
    //public class ProductVariantWriteDtoValidator : AbstractValidator<ProductVariantWriteDto>
    //{
    //    public ProductVariantWriteDtoValidator()
    //    {
    //        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
    //        RuleFor(x => x.Size).NotEmpty().MaximumLength(20);
    //        RuleFor(x => x.Sku).NotEmpty().MaximumLength(60);
    //        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    //        RuleFor(x => x.PriceOverride).GreaterThan(0).When(x => x.PriceOverride.HasValue);
    //    }
    //}

    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Fabric).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Pattern).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Mrp).GreaterThan(0).WithMessage("MRP must be greater than 0.");
            RuleFor(x => x.WholesalePrice)
                .GreaterThan(0).WithMessage("Wholesale price must be greater than 0.")
                .LessThanOrEqualTo(x => x.Mrp).WithMessage("Wholesale price cannot exceed MRP.");

            RuleFor(x => x.MinWholesaleQuantity).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);

            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Category is required.");
            RuleFor(x => x.BrandId).GreaterThan(0).WithMessage("Brand is required.");

            //RuleForEach(x => x.Variants).SetValidator(new ProductVariantWriteDtoValidator());
        }
    }

    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            Include(new ProductCreateDtoValidator());
        }
    }
}
