using System.Linq.Expressions;
using ShopApp.Domain.Entities;
using ShopApp.Domain.Models;

namespace ShopApp.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdWithCategoryStatusAsync(int id, params Expression<Func<Product, object>>[] includes);
    Task<List<Product>> GetAllWithCategoryStatusAsync(params Expression<Func<Product, object>>[] includes);
    Task<Product?> GetByIdAsync(int id, params Expression<Func<Product, object>>[] includes);
    Task<List<Product>> GetAllAsync(params Expression<Func<Product, object>>[] includes);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);

    Task SoftDeleteAsync(int id);

    Task DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);

    IQueryable<Product> GetQueryable();

    Task<List<Product>> FindAsync(Expression<Func<Product, bool>> predicate,
        params Expression<Func<Product, object>>[] includes);

    Task<PaginatedResponse<Product>> GetPaginatedAsync(Expression<Func<Product, bool>> predicate, int pageNumber,
        int pageSize, params Expression<Func<Product, object>>[] includes);
}