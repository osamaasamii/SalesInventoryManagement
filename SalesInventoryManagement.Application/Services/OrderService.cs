using AutoMapper;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Domain.Entities;
using SalesInventoryManagement.Application.Exceptions;

namespace SalesInventoryManagement.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id);

            return order is null
                ? null
                : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            // 1. التأكد إن العميل موجود
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);

            if (customer is null)
                throw new NotFoundException("Customer not found");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Status = OrderStatus.Pending
            };

            // 2. المرور على كل Product في الـ Order
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                // Product غير موجود
                if (product is null)
                {
                    throw new NotFoundException(
                        $"Product with id {item.ProductId} not found");
                }

                // Stock غير كافي
                if (product.StockQuantity < item.Quantity)
                {
                    throw new BusinessRuleException(
                        $"الكمية المطلوبة من '{product.Name}' غير متوفرة. المتاح: {product.StockQuantity}");
                }

                // إضافة OrderItem
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                // تقليل الـ Stock
                product.StockQuantity -= item.Quantity;

                _unitOfWork.Products.Update(product);
            }

            // إضافة Order
            await _unitOfWork.Orders.AddAsync(order);

            // حفظ كل التغييرات
            await _unitOfWork.SaveChangesAsync();

            // إعادة تحميل الـ Order بالتفاصيل
            var createdOrder =
                await _unitOfWork.Orders.GetByIdWithDetailsAsync(order.Id);

            return _mapper.Map<OrderDto>(createdOrder);
        }
    }
}