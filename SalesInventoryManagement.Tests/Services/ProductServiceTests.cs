using AutoMapper;
using Moq;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Exceptions;
using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Application.Services;
using SalesInventoryManagement.Domain.Entities;
using Xunit;

namespace SalesInventoryManagement.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IGenericRepository<Category>> _categoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepoMock = new Mock<IGenericRepository<Product>>();
            _categoryRepoMock = new Mock<IGenericRepository<Category>>();
            _mapperMock = new Mock<IMapper>();

            // لما حد ينادي على _unitOfWork.Products، ارجعله الـ Mock بتاع الـ Repository
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);

            _service = new ProductService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductNotFound_ShouldReturnNull()
        {
            // Arrange: لو حد سأل عن أي id، ارجع null (يعني مش موجود)
            _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WhenCategoryNotFound_ShouldThrowNotFoundException()
        {
            // Arrange: التصنيف مش موجود
            _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Category?)null);

            var dto = new CreateProductDto
            {
                Name = "Laptop",
                Price = 15000,
                StockQuantity = 5,
                CategoryId = 99
            };

            // Act & Assert: نتأكد إن الميثود بترمي الـ Exception الصح
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCallSaveChangesOnce()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Electronics" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());

            var dto = new CreateProductDto
            {
                Name = "Laptop",
                Price = 15000,
                StockQuantity = 5,
                CategoryId = 1
            };

            // Act
            await _service.CreateAsync(dto);

            // Assert: نتأكد إن SaveChangesAsync اتنادى مرة واحدة بالظبط، مش صفر ولا مرتين
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}