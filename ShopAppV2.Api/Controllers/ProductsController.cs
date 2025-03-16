using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Dtos;
using ShopApp.Application.Services;
using ShopApp.Domain.Models;

namespace ShopAppV2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    [HttpGet]
    public async Task<Response<List<ProductDto>>> GetProducts()
    {
        var response = await _productService.GetAllProductsAsync();
        return response;
    }

    // GET: api/products/paginate
    [HttpGet("Paginate")]
    public async Task<Response<PaginatedResponse<ProductDto>>> GetPaginatedProducts([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequestDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await _productService.GetPaginatedProductsAsync(pagination);
        return response;
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<Response<ProductDto>> GetProductById(int id)
    {
        var response = await _productService.GetProductByIdAsync(id);
        return response;
    }

    // POST: api/products/create
    [Authorize]
    [HttpPost("Create")]
    [Consumes("multipart/form-data")]
    public async Task<Response<ProductDto>> CreateProduct(ProductFormDto productFormDto)
    {
        var response = await _productService.CreateProductAsync(productFormDto);
        return response;
    }

    [Authorize]
    // PUT: api/products/{id}
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<Response<ProductDto>> UpdateProduct(int id, ProductFormDto productFormDto)
    {
        var response = await _productService.UpdateProductAsync(id, productFormDto);
        return response;
    }

    [Authorize(Roles = "Admin")]
    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<Response> DeleteProduct(int id)
    {
        var response = await _productService.DeleteProductAsync(id);
        return response;
    }
}