namespace ShopApp.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public double Price { get; set; }

    public int Rating { get; set; }

    public string? ImagePath { get; set; }
}