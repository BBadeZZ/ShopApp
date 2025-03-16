using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Entities;
using ShopApp.Domain.Extensions;
using ShopApp.Domain.Models;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Infrastructure.Repositories;

public class EfGenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly EfDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public EfGenericRepository(EfDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes) query = query.Include(include);

        return await query.FirstOrDefaultAsync(e =>
            EF.Property<int>(e, "Id") == id && EF.Property<Status>(e, "Status") == Status.Active);
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Apply includes
        foreach (var include in includes) query = query.Include(include);

        return await query.Where(e => e.Status == Status.Active).ToListAsync();
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Apply includes
        foreach (var include in includes) query = query.Include(include);

        return await query.Where(predicate).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Apply includes
        foreach (var include in includes) query = query.Include(include);

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            // entity.Status = Status.Deleted;
            // entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(true);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && e.Status == Status.Active);
    }

    public IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }


    public async Task<PaginatedResponse<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicate, int pageNumber,
        int pageSize, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(predicate);
        foreach (var include in includes) query = query.Include(include);
        return await query.ToPaginatedResponseAsync(pageNumber, pageSize);
    }
}