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

    public static CategoryTreeDto BuildTreeFromGraph(int root, CategoryDto rootCategory, Dictionary<int, List<CategoryDto>> graph)
    {
        var tree = new CategoryTreeDto
        {
            Id = root,
            Name = rootCategory.Name,
            Path = rootCategory.Path,
        };

        if (graph.TryGetValue(root, out List<CategoryDto>? children))
        {
            foreach (var child in children)
            {
                tree.Children.Add(BuildTreeFromGraph(child.Id, child, graph));
            }
        }

        return tree;
    }
}