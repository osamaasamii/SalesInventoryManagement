using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Domain.Entities;
using SalesInventoryManagement.Infrastructure.Data;

namespace SalesInventoryManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new GenericRepository<Product>(_context);
            Categories = new GenericRepository<Category>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Orders = new OrderRepository(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
        }

        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Customer> Customers { get; }
       
        public IOrderRepository Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }


        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}