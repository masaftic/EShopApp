using EShopApp.Application.Payments.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using EShopApp.Infrastructure.Data;

namespace EShopApp.Tests.Testing;

public static class TestDataHelper
{
    public static async Task<User> CreateTestUser(ApplicationDbContext context, int id = 1)
    {
        var user = new User(id, "Test", "User", "test@example.com", new Address("st", "city", "country"));
        context.DomainUsers.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public static async Task<(Product Product, Category Category)> CreateTestProduct(
        ApplicationDbContext context,
        int productId = 1,
        decimal price = 10.0m,
        int stockQuantity = 10)
    {
        var category = new Category("Test Category");
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product(productId, "Test Product", price, "Description", category.Id);
        var inventory = new Inventory(productId, product, stockQuantity, 100, 1);
        product.Inventory = inventory;
        
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        return (product, category);
    }

    public static async Task<Cart> CreateTestCart(ApplicationDbContext context, int userId = 1)
    {
        var cart = new Cart(userId);
        await context.Carts.AddAsync(cart);
        await context.SaveChangesAsync();
        return cart;
    }

    public static async Task<CartItem> CreateTestCartItem(
        ApplicationDbContext context, 
        int cartId,
        int productId,
        int quantity = 1,
        decimal unitPrice = 10.0m)
    {
        var cartItem = new CartItem(cartId, productId, quantity, unitPrice);
        await context.CartItems.AddAsync(cartItem);
        await context.SaveChangesAsync();
        return cartItem;
    }

    public static async Task<Category> CreateTestCategory(
        ApplicationDbContext context,
        string name = "Test Category",
        string parentPath = "")
    {
        var category = new Category(name);
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        
        category.InitPath(parentPath);
        await context.SaveChangesAsync();
        return category;
    }
}
