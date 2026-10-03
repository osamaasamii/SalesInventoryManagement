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
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Customer>> _customerRepoMock;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _customerRepoMock = new Mock<IGenericRepository<Customer>>();
            _productRepoMock = new Mock<IGenericRepository<Product>>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepoMock.Object);

            _service = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenCustomerNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Customer?)null);

            var dto = new CreateOrderDto
            {
                CustomerId = 999,
                Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 1 } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));

            // مهم: نتأكد إن الحفظ ماحصلش خالص لو فيه خطأ
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenProductNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var customer = new Customer { Id = 1, FullName = "Osama" };
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product?)null);

            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemDto> { new() { ProductId = 999, Quantity = 1 } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenStockInsufficient_ShouldThrowBusinessRuleException()
        {
            // Arrange
            var customer = new Customer { Id = 1, FullName = "Osama" };
            var product = new Product { Id = 1, Name = "Laptop", Price = 15000, StockQuantity = 5 };

            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 999 } } // أكبر من المتاح
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateAsync(dto));
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReduceStockAndSaveOnce()
        {
            // Arrange
            var customer = new Customer { Id = 1, FullName = "Osama" };
            var product = new Product { Id = 1, Name = "Laptop", Price = 15000, StockQuantity = 10 };

            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _orderRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Order { Id = 1, Customer = customer, OrderItems = new List<OrderItem>() });
            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(new OrderDto());

            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 3 } }
            };

            // Act
            await _service.CreateAsync(dto);

            // Assert
            Assert.Equal(7, product.StockQuantity); // 10 - 3 = 7
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}