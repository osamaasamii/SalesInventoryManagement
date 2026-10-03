using Microsoft.EntityFrameworkCore;
using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Domain.Entities;
using SalesInventoryManagement.Infrastructure.Data;


namespace SalesInventoryManagement.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Order>> GetAllWithDetailsAsync() =>
            await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

        public async Task<Order?> GetByIdWithDetailsAsync(int id) =>
            await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
    }
}