using ShopApp.Application.Dtos;
using ShopApp.Domain.Models;

namespace ShopApp.Application.Services;

public interface IProductService
{
    Task<Response<List<ProductDto>>> GetAllProductsAsync();
    Task<Response<ProductDto>> GetProductByIdAsync(int id);
    Task<Response<ProductDto>> CreateProductAsync(ProductFormDto productFormDto);
    Task<Response<ProductDto>> UpdateProductAsync(int id, ProductFormDto productFormDto);
    Task<Response> DeleteProductAsync(int id);

    Task<Response<PaginatedResponse<ProductDto>>> GetPaginatedProductsAsync(PaginationRequestDto request);
}