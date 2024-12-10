namespace EShopApp.Application.Common.DTOs;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageSize { get; }
    public int PageNumber { get; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    
    public PaginatedList(IReadOnlyList<T> items, int pageSize, int pageNumber)
    {
        Items = items;
        TotalCount = items.Count;
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
}