namespace ShopApp.Domain.Models;

public class PaginatedResponse<T>
{
    public PaginatedResponse(List<T> data, int totalItems, int pageNumber, int pageSize)
    {
        Data = data;
        TotalItems = totalItems;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    public List<T> Data { get; set; } = new();
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}