namespace EShopApp.Application.Categories.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public int? ParentId { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}