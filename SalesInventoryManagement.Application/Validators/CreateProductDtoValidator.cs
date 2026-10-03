using FluentValidation;
using SalesInventoryManagement.Application.DTOs;

namespace SalesInventoryManagement.Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المنتج مطلوب")
                .MaximumLength(100).WithMessage("اسم المنتج لازم يكون أقل من 100 حرف");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("السعر لازم يكون أكبر من صفر");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("الكمية متقدرش تكون بالسالب");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("لازم تحدد تصنيف صحيح للمنتج");
        }
    }
}