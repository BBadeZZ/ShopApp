using System.Linq.Expressions;
using ShopApp.Domain.Models;

namespace ShopApp.Infrastructure.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task SoftDeleteAsync(int id);
    Task DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);

    IQueryable<T> GetQueryable();

    Task<PaginatedResponse<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize,
        params Expression<Func<T, object>>[] includes);
}