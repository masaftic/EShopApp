using Bogus;
using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public DataSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task MigrateAsync()
    {
        await _context.Database.MigrateAsync();
    }

    public async Task SetUpRoles()
    {
        var roleExists = await _roleManager.RoleExistsAsync("Admin");
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
        }
    }

    public async Task CreateAdminAsync()
    {
        var admin = new User("Admin", "Admin", "Admin@gmail.com");
        if (await _userManager.FindByEmailAsync("admin@gmail.com") == null)
        {
            var adminUser = new ApplicationUser(admin);
            await _userManager.CreateAsync(adminUser, "Admin123!");
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    public async Task SeedAsync()
    {
        await CreateAdminAsync();
        await SeedCategoriesAsync();
        await SeedProductsAndInventoriesAsync(100);
    }

    private async Task SeedCategoriesAsync()
    {
        if (await _context.Categories.AnyAsync())
            return;

        var categories = new List<Category>();

        // var categoryDefinitions = new Dictionary<string, string[]>
        // {
        //     { "Electronics", ["Computers", "Smartphones", "Cameras"] },
        //     { "Books", ["Fiction", "Non-Fiction", "Children's Books"] },
        //     { "Clothing", ["Men", "Women", "Kids"] },
        //     { "Home & Kitchen", ["Furniture", "Appliances", "Decor"] },
        //     { "Sports", ["Gym Equipment", "Outdoor", "Team Sports"] },
        //     { "Beauty", ["Skincare", "Makeup", "Haircare"] },
        //     { "Toys", ["Educational", "Action Figures", "Dolls"] },
        //     { "Automotive", ["Parts", "Accessories", "Tools"] },
        //     { "Garden", ["Plants", "Tools", "Outdoor Lighting"] },
        //     { "Health", ["Supplements", "Medical Supplies", "Fitness"] },
        //     // Add more parent-subcategory relationships as needed
        // };

        // var id = 1; // simulating Id

        // foreach (var (name, children) in categoryDefinitions)
        // {
        //     var root = new Category(name);
        //     var rootPath = $"/{id}";
        //     root.SetIdAndPath(id, rootPath);

        //     categories.Add(root);

        //     foreach (var t in children)
        //     {
        //         id += 1;
        //         var child = new Category(t);
        //         child.SetIdAndPath(id, $"{rootPath}/{id}");

        //         categories.Add(child);
        //     }

        //     id += 1;
        // }

        categories.Add(new Category("my category"));

        for (int i = 0; i < 100; i++) {
            Category? parent = Random.Shared.NextDouble() < 0.2 ? null : categories[Random.Shared.Next(categories.Count)];

            var category = new CategoryFaker(parent).Generate();  
            categories.Add(category);
        }

        await _context.AddRangeAsync(categories);
        await _context.SaveChangesAsync();

        foreach (var category in categories)
        {
            category.UpdatePath();
        }

        await _context.SaveChangesAsync();

        // await using var transaction = await _context.Database.BeginTransactionAsync();

        // try
        // {
        //     // Enable IDENTITY_INSERT
        //     await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Categories ON");

        //     // Insert categories with explicit IDs
        //     await _context.Categories.AddRangeAsync(categories);
        //     await _context.SaveChangesAsync();

        //     // Disable IDENTITY_INSERT
        //     await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Categories OFF");

        //     // Commit the transaction
        //     await transaction.CommitAsync();
        // }
        // catch
        // {
        //     // Rollback the transaction in case of an error
        //     await transaction.RollbackAsync();
        //     throw;
        // }
    }

    // There must be Product and Inventory parameterless constructors before running this function. 
    private async Task SeedProductsAndInventoriesAsync(int count)
    {
        if (await _context.Products.AnyAsync())
            return;

        var categories = await _context.Categories.ToListAsync();  

        var products = new List<Product>();
        var inventories = new List<Inventory>();

        for (var i = 0; i < count; i++)
        {
            var category = categories[new Random().Next(categories.Count)];
            var product = new ProductFaker(category).Generate();

            products.Add(product);
        }

        await _context.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var productIds = products.Select(p => p.Id).ToList();
        for (var i = 0; i < count; i++)
        {
            var inventory = new InventoryFaker(productIds[i]).Generate();
            inventories.Add(inventory);
        }

        await _context.AddRangeAsync(inventories);
        await _context.SaveChangesAsync();
    }
}

class ProductFaker : Faker<Product>
{
    public ProductFaker(Category category)
    {
        CustomInstantiator(f => new Product(
            name: f.Name.FullName(),
            price: f.Random.Decimal(1, 100),
            description: f.Lorem.Sentence(),
            category: category));
    }
}

public class InventoryFaker : Faker<Inventory>
{
    public InventoryFaker(int productId)
    {
        CustomInstantiator(f => new Inventory(
            productId: productId,
            stock: f.Random.Number(1, 100),
            reorderQuantity: f.Random.Number(5, 20),
            reorderLevel: f.Random.Number(5, 20)));
    }
}

public class CategoryFaker : Faker<Category>
{
    public CategoryFaker(Category? parent)
    {
        if (parent is null)
        {
            CustomInstantiator(f => new Category($"{f.Commerce.Categories(1)[0]}-{f.Random.AlphaNumeric(5)}"));
        }
        else 
        {
            CustomInstantiator(f => new Category($"{f.Commerce.Categories(1)[0]}-{f.Random.AlphaNumeric(5)}", parent));
        }
    }
}