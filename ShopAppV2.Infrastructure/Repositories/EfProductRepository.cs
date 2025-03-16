using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Entities;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Infrastructure.Repositories;

public class EfProductRepository : EfGenericRepository<Product>, IProductRepository
{
    private readonly EfDbContext _dbContext;

    public EfProductRepository(EfDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdWithCategoryStatusAsync(int id,
        params Expression<Func<Product, object>>[] includes)
    {
        IQueryable<Product> query = _dbContext.Products;

        // Include additional related entities
        foreach (var include in includes) query = query.Include(include);

        // Check both product's status and category's status
        return await query
            .Where(p => p.Id == id && p.Status == Status.Active && p.Category.Status == Status.Active)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetAllWithCategoryStatusAsync(params Expression<Func<Product, object>>[] includes)
    {
        IQueryable<Product> query = _dbContext.Products;

        // Include additional related entities
        foreach (var include in includes) query = query.Include(include);

        // Filter products where both the product and category are active
        return await query
            .Where(p => p.Status == Status.Active && p.Category.Status == Status.Active)
            .ToListAsync();
    }
}