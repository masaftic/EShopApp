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
            
            var cart = new Cart(admin.Id);
            var wishlist = new Wishlist(admin.Id);
            var address = new Address("456 Elm Street", "Suite 200", "Los Angeles", "CA", "12345");
            admin.UpdateAddress(address);
            await _context.AddAsync(wishlist);
            await _context.AddAsync(cart);
            await _context.SaveChangesAsync();
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

        var categories = new List<Category>
        {
            new("my category")
        };

        for (int i = 0; i < 100; i++)
        {
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
    }


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
        CustomInstantiator(f =>
        {
            var p = new Product(
            name: f.Commerce.ProductName(),
            price: f.Random.Decimal(1, 100),
            description: f.Commerce.ProductDescription(),
            category: category);

            p.IncreaseSoldAmount(f.Random.Number(1, 100));

            foreach (var _ in Enumerable.Range(0, f.Random.Number(1, 5)))
            {
                p.AddReview(new ProductReview(
                    productId: p.Id,
                    userId: 1,
                    comment: f.Lorem.Sentence(),
                    rating: f.Random.Number(1, 5)));
            }

            return p;
        });
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
            CustomInstantiator(f => new Category($"{f.Commerce.Categories(1)[0]}"));
        }
        else
        {
            CustomInstantiator(f => new Category($"{f.Commerce.Categories(1)[0]}", parent));
        }
    }
}