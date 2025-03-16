using ShopApp.Domain.Entities;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Application.Services;

public class CategoryManagerService : ICategoryManagerService
{
    private readonly AdoDbContext _adoDbContext;

    public CategoryManagerService(AdoDbContext adoDbContext)
    {
        _adoDbContext = adoDbContext;
    }

    public async Task<string> GetCategoryNameById(int id)
    {
        var query = $"SELECT * FROM public.\"Categories\" WHERE id = {id} ";
        var category = await _adoDbContext.ExecuteSingleQueryAsync(query, reader => new Category
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name"))
        });

        return category != null ? category.Name : "N/A";
    }
}