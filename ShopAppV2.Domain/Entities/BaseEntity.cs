namespace ShopApp.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Status Status { get; set; } = Status.Active;

    public DateTime? DeletedAt { get; set; }

    public int CreatedBy { get; set; }

    public int LastUpdatedBy { get; set; }

    public int DeletedBy { get; set; }
}