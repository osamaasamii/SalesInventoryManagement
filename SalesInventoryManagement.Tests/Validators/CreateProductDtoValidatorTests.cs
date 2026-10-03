using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Validators;
using Xunit;

namespace SalesInventoryManagement.Tests.Validators
{
    public class CreateProductDtoValidatorTests
    {
        private readonly CreateProductDtoValidator _validator;

        public CreateProductDtoValidatorTests()
        {
            _validator = new CreateProductDtoValidator();
        }

        [Fact]
        public void Validate_WithValidData_ShouldReturnValid()
        {
            var dto = new CreateProductDto
            {
                Name = "Laptop",
                Price = 15000,
                StockQuantity = 10,
                CategoryId = 1
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WithEmptyName_ShouldReturnInvalid()
        {
            var dto = new CreateProductDto
            {
                Name = "",
                Price = 15000,
                StockQuantity = 10,
                CategoryId = 1
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Theory]
        [InlineData(-50)]
        [InlineData(0)]
        public void Validate_WithInvalidPrice_ShouldReturnInvalid(decimal invalidPrice)
        {
            var dto = new CreateProductDto
            {
                Name = "Laptop",
                Price = invalidPrice,
                StockQuantity = 10,
                CategoryId = 1
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        }
    }
}