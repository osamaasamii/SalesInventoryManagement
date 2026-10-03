using FluentValidation;
using SalesInventoryManagement.Application.DTOs;

namespace SalesInventoryManagement.Application.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("لازم تحدد عميل صحيح");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("الأوردر لازم يحتوي على منتج واحد على الأقل");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateOrderItemDtoValidator());
        }
    }
}