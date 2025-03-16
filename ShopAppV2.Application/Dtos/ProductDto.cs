namespace ShopApp.Application.Dtos;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public int StockQuantity { get; set; }

    public CategoryDto Category { get; set; } = null!;

    public double Price { get; set; }

    public int Rating { get; set; }

    public string? ImagePath { get; set; }

    public int CreatedBy { get; set; }

    public int LastUpdatedBy { get; set; }
}