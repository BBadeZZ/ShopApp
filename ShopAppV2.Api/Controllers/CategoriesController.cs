using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Dtos;
using ShopApp.Application.Services;
using ShopApp.Domain.Models;

namespace ShopAppV2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }


    // GET: api/categories
    [HttpGet]
    public async Task<Response<List<CategoryDto>>> GetCategories()
    {
        var response = await _categoryService.GetAllCategoriesAsync();
        return response;
    }

    // GET: api/categories/{id}
    [HttpGet("{id}")]
    public async Task<Response<CategoryDto>> GetCategoryById(int id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);
        return response;
    }

    // GET: api/categories/paginate
    [HttpPost("Paginate")]
    public async Task<Response<PaginatedResponse<CategoryDto>>> GetPaginatedCategory([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequestDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await _categoryService.GetPaginatedCategoriesAsync(pagination);
        return response;
    }

    // POST: api/categories/create
    [Authorize]
    [HttpPost("Create")]
    public async Task<Response<CategoryDto>> CreateCategory(CategoryFormDto categoryFormDto)
    {
        var response = await _categoryService.CreateCategoryAsync(categoryFormDto);
        return response;
    }

    [Authorize]
    // PUT: api/categories/{id}
    [HttpPut("{id}")]
    public async Task<Response<CategoryDto>> UpdateCategory(int id, CategoryFormDto categoryFormDto)
    {
        var response = await _categoryService.UpdateCategoryAsync(id, categoryFormDto);
        return response;
    }

    [Authorize]
    // DELETE: api/categories/{id}
    [HttpDelete("{id}")]
    public async Task<Response> DeleteCategory(int id)
    {
        var response = await _categoryService.DeleteCategoryAsync(id);
        return response;
    }
}