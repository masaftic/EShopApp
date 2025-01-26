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
        var admin = new User("Admin", "Admin", "Admin@gmail.com", new Address("Street 123", "City 123", "Country 123"));
        if (await _userManager.FindByEmailAsync("admin@gmail.com") == null)
        {
            var adminUser = ApplicationUser.FromUser(admin);
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

        var categoryDefinitions = new Dictionary<string, string[]>
        {
            { "Electronics", ["Computers", "Smartphones", "Cameras"] },
            { "Books", ["Fiction", "Non-Fiction", "Children's Books"] },
            { "Clothing", ["Men", "Women", "Kids"] },
            { "Home & Kitchen", ["Furniture", "Appliances", "Decor"] },
            { "Sports", ["Gym Equipment", "Outdoor", "Team Sports"] },
            { "Beauty", ["Skincare", "Makeup", "Haircare"] },
            { "Toys", ["Educational", "Action Figures", "Dolls"] },
            { "Automotive", ["Parts", "Accessories", "Tools"] },
            { "Garden", ["Plants", "Tools", "Outdoor Lighting"] },
            { "Health", ["Supplements", "Medical Supplies", "Fitness"] },
            // Add more parent-subcategory relationships as needed
        };

        var id = 1; // simulating Id

        foreach (var (name, children) in categoryDefinitions)
        {
            var root = new Category(name);
            var rootPath = $"/{id}";
            root.SetIdAndPath(id, rootPath);

            categories.Add(root);

            foreach (var t in children)
            {
                id += 1;
                var child = new Category(t);
                child.SetIdAndPath(id, $"{rootPath}/{id}");

                categories.Add(child);
            }

            id += 1;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Enable IDENTITY_INSERT
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Categories ON");

            // Insert categories with explicit IDs
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Disable IDENTITY_INSERT
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Categories OFF");

            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch
        {
            // Rollback the transaction in case of an error
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task SeedProductsAndInventoriesAsync(int count)
    {
        if (await _context.Products.AnyAsync())
            return;
        
        var categoryIds = await _context.Categories.Select(c => c.Id).ToListAsync();

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 100))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(2))
            .RuleFor(p => p.UpdatedAt, (f, u) => u.CreatedAt)
            .RuleFor(p => p.CategoryId, f => f.PickRandom(categoryIds));

        var inventoryFaker = new Faker<Inventory>()
            .RuleFor(i => i.Stock, f => f.Random.Number(1, 100))
            .RuleFor(i => i.ReservedStock, f => f.Random.Number(1, 10))
            .RuleFor(i => i.ReorderLevel, f => f.Random.Number(5, 20))
            .RuleFor(i => i.ReorderQuantity, f => f.Random.Number(5, 20));
        
        var products = new List<Product>();
        var inventories = new List<Inventory>();
        
        for (var i = 0; i < count; i++)
        {
            var product = productFaker.Generate();
            var inventory = inventoryFaker.Generate();
            product.Inventory = inventory;
            inventory.Product = product;
            products.Add(product);
            inventories.Add(inventory);
        }

        await _context.AddRangeAsync(products);
        await _context.AddRangeAsync(inventories);
        
        await _context.SaveChangesAsync();
    }
    
}