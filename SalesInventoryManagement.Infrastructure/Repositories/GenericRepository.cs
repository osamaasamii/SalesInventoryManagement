using Microsoft.EntityFrameworkCore;
using SalesInventoryManagement.Application.Interfaces;
using SalesInventoryManagement.Domain.Entities;
using SalesInventoryManagement.Infrastructure.Data;

namespace SalesInventoryManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id) =>
            await _context.Set<T>().FindAsync(id);

        public async Task<List<T>> GetAllAsync() =>
            await _context.Set<T>().ToListAsync();

        public async Task AddAsync(T entity) =>
            await _context.Set<T>().AddAsync(entity);

        public void Update(T entity) =>
            _context.Set<T>().Update(entity);

        public void Delete(T entity) =>
            _context.Set<T>().Remove(entity);
    }
}