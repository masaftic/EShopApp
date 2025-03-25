using EShopApp.Application.Products.Commands.Add;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using EShopApp.Tests.Testing;

namespace EShopApp.Tests.Application.Products;

public class AddProductHandlerTests : TestBase
{
    private readonly AddProductCommandHandler _handler;

    public AddProductHandlerTests()
    {
        _handler = new AddProductCommandHandler(DbContext);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCategoryNotFound()
    {
        // Arrange
        var command = new AddProductCommand
        (
            Name: "Test Product",
            Price: 100,
            Description: "Test Description",
            CategoryId: 1
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DomainErrors.Category.NotFound(command.CategoryId), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldAddProduct_WhenCategoryExists()
    {
        // Arrange
        var category = new Category("Test Category");
        await DbContext.Categories.AddAsync(category);
        await DbContext.SaveChangesAsync();

        var command = new AddProductCommand
        (
            Name: "Test Product",
            Price: 100,
            Description: "Test Description",
            CategoryId: category.Id
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.Price, result.Value.Price);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.CategoryId, result.Value.CategoryId);

        var savedProduct = await DbContext.Products.FindAsync(result.Value.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(command.Name, savedProduct.Name);
    }
}