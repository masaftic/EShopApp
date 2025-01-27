using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Products.Commands.Add;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace EShopApp.Tests.ApplicationTests.Products.Commands;

public class AddProductHandlerTests
{
    private readonly IApplicationDbContext _mockDbContext;
    private readonly AddProductCommandHandler _handler;

    public AddProductHandlerTests()
    {
        _mockDbContext = Substitute.For<IApplicationDbContext>();
        _handler = new AddProductCommandHandler(_mockDbContext);
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

        var productsDbSet = Substitute.For<DbSet<Product>, IQueryable<Product>>();
        var categoriesDbSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();

        _mockDbContext.Categories.Returns(categoriesDbSet);
        _mockDbContext.Products.Returns(productsDbSet);
        
        categoriesDbSet.FindAsync(Arg.Is<object[]>(keys => keys.Contains(command.CategoryId)), Arg.Any<CancellationToken>())
            .Returns((Category)null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(Errors.Category.NotFound(command.CategoryId), result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldAddProduct_WhenCategoryExists()
    {
        // Arrange
        var command = new AddProductCommand
        (
            Name: "Test Product",
            Price: 100,
            Description: "Test Description",
            CategoryId: 1
        );

        var category = new Category("Test Category");
        
        var productsDbSet = Substitute.For<DbSet<Product>, IQueryable<Product>>();
        var categoriesDbSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();

        _mockDbContext.Categories.Returns(categoriesDbSet);
        _mockDbContext.Products.Returns(productsDbSet);
        
        categoriesDbSet.FindAsync(Arg.Is<object[]>(keys => keys.Contains(command.CategoryId)), Arg.Any<CancellationToken>())
            .Returns(category);
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

  
        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.Price, result.Value.Price);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.CategoryId, result.Value.CategoryId);

        await _mockDbContext.Products.Received(1).AddAsync(Arg.Is<Product>(p => p.Name == command.Name), Arg.Any<CancellationToken>());
        await _mockDbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}