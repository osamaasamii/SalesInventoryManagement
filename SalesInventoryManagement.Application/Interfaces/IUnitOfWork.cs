using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Domain.Entities;

public interface IUnitOfWork
{
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Customer> Customers { get; }
    IOrderRepository Orders { get; }
    IGenericRepository<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync();
}