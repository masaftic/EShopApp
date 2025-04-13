using Spectre.Console;

namespace EShopApp.Application.Categories.DTOs;

public class CategoryTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public List<CategoryTreeDto> Children { get; set; } = [];

    private CategoryTreeDto()
    {
    }

    public static CategoryTreeDto BuildTreeFromGraph(CategoryDto current, Dictionary<int, List<CategoryDto>> graph)
    {
        var tree = new CategoryTreeDto
        {
            Id = current.Id,
            Name = current.Name,
            Path = current.Path,
        };

        if (graph.TryGetValue(current.Id, out List<CategoryDto>? children))
        {
            foreach (var child in children)
            {
                tree.Children.Add(BuildTreeFromGraph(child, graph));
            }
        }

        return tree;
    }
}