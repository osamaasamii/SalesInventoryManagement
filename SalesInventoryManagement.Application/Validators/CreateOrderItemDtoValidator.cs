using FluentValidation;
using SalesInventoryManagement.Application.DTOs;


namespace SalesInventoryManagement.Application.Validators
{
    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("لازم تحدد منتج صحيح");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("الكمية لازم تكون أكبر من صفر");
        }
    }
}