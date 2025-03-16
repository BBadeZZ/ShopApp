using AutoMapper;
using ShopApp.Application.Dtos;
using ShopApp.Domain.Entities;
using ShopApp.Domain.Models;
using ShopApp.Infrastructure.Repositories;

namespace ShopApp.Application.Services;

public class EfProductService : IProductService
{
    private readonly string _cacheKey = "products_list";
    private readonly ICacheService _cacheService;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IImageUploaderService _imageUploaderService;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;

    public EfProductService(IMapper mapper, IImageUploaderService imageUploaderService,
        IProductRepository productRepository, IGenericRepository<Category> categoryRepository,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _imageUploaderService = imageUploaderService;
        _cacheService = cacheService;
    }

    public async Task<Response<List<ProductDto>>> GetAllProductsAsync()
    {
        try
        {
            // Check cache first
            var cachedProducts = await _cacheService.GetAsync<List<ProductDto>>(_cacheKey);
            if (cachedProducts != null)
                return Response<List<ProductDto>>.Success("Ok, returned from cache.", cachedProducts);


            var products = await _productRepository.GetAllWithCategoryStatusAsync(p => p.Category);

            var productsDto = _mapper.Map<List<ProductDto>>(products);

            await _cacheService.SetAsync(_cacheKey, _mapper.Map<List<ProductDto>>(products), TimeSpan.FromMinutes(30));

            return productsDto.Count != 0
                ? Response<List<ProductDto>>.Success("Ok", productsDto)
                : Response
                    <List<ProductDto>>.Success("No products found!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<List<ProductDto>>.Fail("An error occured! Please try again!");
        }
    }

    public async Task<Response<ProductDto>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdWithCategoryStatusAsync(id, p => p.Category);

            var productDto = _mapper.Map<ProductDto>(product);

            return product != null
                ? Response<ProductDto>.Success("Ok", productDto)
                : Response<ProductDto>.Success("Product not found!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<ProductDto>.Fail("Error occured getting the product! Please try again!");
        }
    }

    public async Task<Response<ProductDto>> CreateProductAsync(ProductFormDto productFormDto)
    {
        var product = _mapper.Map<Product>(productFormDto);

        // Upload the image
        try
        {
            product.ImagePath = productFormDto.Image != null
                ? await _imageUploaderService.UploadImage(productFormDto.Image)
                : "";
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error uploading image: {e}");
        }

        // Check category id is correct
        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        if (category == null)
            return Response<ProductDto>.Fail("Failed to create product! Category does not exist!");

        try
        {
            await _productRepository.AddAsync(product);
            await _cacheService.RemoveAsync(_cacheKey);
            product = await _productRepository.GetByIdWithCategoryStatusAsync(product.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating product: {e.Message}");
            return Response<ProductDto>.Fail("Failed to create product!");
        }

        return Response<ProductDto>.Success("Ok", _mapper.Map<ProductDto>(product));
    }


    public async Task<Response<ProductDto>> UpdateProductAsync(int id, ProductFormDto productFormDto)
    {
        // Check that the product exists
        var product = await _productRepository.GetByIdWithCategoryStatusAsync(id);
        var imagePath = "";
        if (product == null)
            return Response<ProductDto>.Fail("Failed to update product! Product does not exist!");

        try
        {
            imagePath = productFormDto.Image != null
                ? await _imageUploaderService.UploadImage(productFormDto.Image)
                : "";
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error uploading image: {e}");
        }

        // Check category id is correct
        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        if (category == null)
            return Response<ProductDto>.Fail("Failed to update product! Category does not exist!");

        try
        {
            // Have a look at here, is there a more compact way to do this, mapper breaks here since product is an EF object
            // but there should be a better way
            product.ImagePath = imagePath;
            product.Name = productFormDto.Name;
            product.Description = productFormDto.Description;
            product.Price = productFormDto.Price;
            product.StockQuantity = productFormDto.StockQuantity;
            product.CategoryId = productFormDto.CategoryId;
            product.Rating = productFormDto.Rating;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _cacheService.RemoveAsync(_cacheKey);

            product = await _productRepository.GetByIdWithCategoryStatusAsync(id, p => p.Category);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating product: {e.Message}");
            Response<ProductDto>.Fail("Failed to update product!");
        }

        return Response<ProductDto>.Success("Ok", _mapper.Map<ProductDto>(product));
    }

    public async Task<Response> DeleteProductAsync(int id)
    {
        // Check that the product exists
        var product = await _productRepository.GetByIdWithCategoryStatusAsync(id);
        if (product == null) return Response.Fail("Failed to delete product! Product does not exist!");

        try
        {
            await _productRepository.SoftDeleteAsync(id);
            await _cacheService.RemoveAsync(_cacheKey);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting product: {e.Message}");
            Response<ProductDto>.Fail("Failed to delete product!");
        }

        return Response.Success("Ok");
    }

    public async Task<Response<PaginatedResponse<ProductDto>>> GetPaginatedProductsAsync(PaginationRequestDto request)
    {
        var paginatedResult = await _productRepository.GetPaginatedAsync(p => p.Status == Status.Active,
            request.PageNumber, request.PageSize, p => p.Category);

        var productDtos = paginatedResult.Data.Select(p => _mapper.Map<ProductDto>(p)).ToList();

        return Response<PaginatedResponse<ProductDto>>.Success("Ok", new PaginatedResponse<ProductDto>(
            productDtos,
            paginatedResult.TotalItems,
            request.PageNumber,
            request.PageSize
        ));
    }
}