using AutoMapper;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Domain.Entities;

namespace SalesInventoryManagement.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<Customer, CustomerDto>();

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.LineTotal,
                    opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OrderDate,
                    opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)))
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems));
        }
    }
}