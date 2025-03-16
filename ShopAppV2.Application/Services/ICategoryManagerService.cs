namespace ShopApp.Application.Services;

public interface ICategoryManagerService
{
    Task<string> GetCategoryNameById(int id);
}