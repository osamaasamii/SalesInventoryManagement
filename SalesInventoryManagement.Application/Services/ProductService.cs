using AutoMapper;
using SalesInventoryManagement.Application.DTOs;
using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Domain.Entities;
using SalesInventoryManagement.Application.Exceptions;

namespace SalesInventoryManagement.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            return product is null
                ? null
                : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

            if (category is null)
                throw new NotFoundException("Category not found");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId
            };

            await _unitOfWork.Products.AddAsync(product);

            await _unitOfWork.SaveChangesAsync();

            // ربط الـ Category عشان AutoMapper يقدر يجيب CategoryName
            product.Category = category;

            return _mapper.Map<ProductDto>(product);
        }
    }
}