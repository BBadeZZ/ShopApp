using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShopApp.Domain.Entities;

namespace ShopApp.Infrastructure.Data;

public class EfDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EfDbContext(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _httpContextAccessor = httpContextAccessor;
    }

    // Constructor for design-time
    public EfDbContext(DbContextOptions<EfDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseNpgsql(_connectionString);
    }

    public async Task<int> SaveChangesAsync(bool bypassSoftDelete = false,
        CancellationToken cancellationToken = default)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var currentUserId))
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserId;
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastUpdatedBy = currentUserId;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        if (bypassSoftDelete)
                        {
                            entry.State = EntityState.Deleted;
                            break;
                        }

                        entry.State = EntityState.Modified;
                        entry.Entity.DeletedBy = currentUserId;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.Status = Status.Deleted;
                        break;
                }

        return await base.SaveChangesAsync(cancellationToken);
    }
}