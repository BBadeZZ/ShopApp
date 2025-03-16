using Microsoft.AspNetCore.Http;

namespace ShopApp.Application.Dtos;

public class ProductFormDto
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }

    public double Price { get; set; }

    public int Rating { get; set; }

    public IFormFile? Image { get; set; }
}