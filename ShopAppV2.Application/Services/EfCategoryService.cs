using AutoMapper;
using ShopApp.Application.Dtos;
using ShopApp.Domain.Entities;
using ShopApp.Domain.Models;
using ShopApp.Infrastructure.Repositories;

namespace ShopApp.Application.Services;

public class EfCategoryService : ICategoryService
{
    private readonly string _cacheKey = "categories_list";

    private readonly ICacheService _cacheService;
    private readonly IGenericRepository<Category> _categoryRepository;

    private readonly IMapper _mapper;

    public EfCategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper,
        ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Response<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        try
        {
            // Check cache first
            var cachedCategories = await _cacheService.GetAsync<List<CategoryDto>>(_cacheKey);
            if (cachedCategories != null)
                return Response<List<CategoryDto>>.Success("Ok, returned from cache.", cachedCategories);

            var categories = _mapper.Map<List<CategoryDto>>(await _categoryRepository.GetAllAsync());

            await _cacheService.SetAsync(_cacheKey, _mapper.Map<List<CategoryDto>>(categories),
                TimeSpan.FromMinutes(30));

            return categories.Count != 0
                ? Response<List<CategoryDto>>.Success("Ok", categories)
                : Response<List<CategoryDto>>.Success("No categories found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<List<CategoryDto>>.Fail("An error occured. Please try again.");
        }
    }

    public async Task<Response<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = _mapper.Map<CategoryDto>(await _categoryRepository.GetByIdAsync(id));

            return category != null
                ? Response<CategoryDto>.Success("Ok", category)
                : Response<CategoryDto>.Success("No category found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<CategoryDto>.Fail("An error occured. Please try again.");
        }
    }

    public async Task<Response<CategoryDto>> CreateCategoryAsync(CategoryFormDto categoryFormDto)
    {
        var category = _mapper.Map<Category>(categoryFormDto);
        try
        {
            await _categoryRepository.AddAsync(category);
            await _cacheService.RemoveAsync(_cacheKey);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating category: {e}");
            return Response<CategoryDto>.Fail("Failed to create category");
        }

        return Response<CategoryDto>.Success("Ok", _mapper.Map<CategoryDto>(category));
    }


    public async Task<Response<CategoryDto>> UpdateCategoryAsync(int id, CategoryFormDto categoryFormDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return Response<CategoryDto>.Fail("Category not found");

        try
        {
            category.Name = categoryFormDto.Name;
            await _categoryRepository.UpdateAsync(category);
            await _cacheService.RemoveAsync(_cacheKey);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating category: {e.Message}");
            Response<Category>.Fail("Failed to update category!");
        }

        return Response<CategoryDto>.Success("Ok", _mapper.Map<CategoryDto>(category));
    }

    public async Task<Response> DeleteCategoryAsync(int id)
    {
        // Check that the category exists
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return Response.Fail("Failed to delete category! Category does not exist!");

        try
        {
            await _categoryRepository.SoftDeleteAsync(id);
            await _cacheService.RemoveAsync(_cacheKey);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting category: {e.Message}");
            Response.Fail("Failed to delete category!");
        }

        return Response.Success("Ok");
    }

    public async Task<Response<PaginatedResponse<CategoryDto>>> GetPaginatedCategoriesAsync(
        PaginationRequestDto request)
    {
        var paginatedResult =
            await _categoryRepository.GetPaginatedAsync(c => c.Status == Status.Active, request.PageNumber,
                request.PageSize);

        var categoryDtos = paginatedResult.Data.Select(c => _mapper.Map<CategoryDto>(c)).ToList();

        return Response<PaginatedResponse<CategoryDto>>.Success("Ok", new PaginatedResponse<CategoryDto>(
            categoryDtos,
            paginatedResult.TotalItems,
            request.PageNumber,
            request.PageSize
        ));
    }
}