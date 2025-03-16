using ShopApp.Application.Dtos;
using ShopApp.Domain.Models;

namespace ShopApp.Application.Services;

public interface ICategoryService
{
    Task<Response<List<CategoryDto>>> GetAllCategoriesAsync();
    Task<Response<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<Response<CategoryDto>> CreateCategoryAsync(CategoryFormDto categoryFormDto);
    Task<Response<CategoryDto>> UpdateCategoryAsync(int id, CategoryFormDto categoryFormDto);
    Task<Response> DeleteCategoryAsync(int id);

    Task<Response<PaginatedResponse<CategoryDto>>> GetPaginatedCategoriesAsync(PaginationRequestDto request);
}