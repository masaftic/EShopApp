using EShopApp.Application.Carts.Commands.AddToCart;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using EShopApp.Tests.Testing;
using NSubstitute;

namespace EShopApp.Tests.ApplicationTests.Carts;

public class AddToCartTests : TestBase
{
    private readonly ICurrentUserService _mockCurrentUserService;
    private readonly AddToCartCommandHandler _handler;

    public AddToCartTests()
    {
        _mockCurrentUserService = Substitute.For<ICurrentUserService>();
        _handler = new AddToCartCommandHandler(_mockCurrentUserService, DbContext);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenValidRequest()
    {
        // Arrange
        var userId = 1;
        var quantity = 2;
        
        await TestDataHelper.CreateTestUser(DbContext, userId);
        var (product, _) = await TestDataHelper.CreateTestProduct(DbContext, stockQuantity: 5);
        await TestDataHelper.CreateTestCart(DbContext, userId);
        
        _mockCurrentUserService.UserId.Returns(userId.ToString());

        // Act
        var result = await _handler.Handle(
            new AddToCartCommand(product.Id, quantity), 
            CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(product.Id, result.Value.ProductId);
        Assert.Equal(quantity, result.Value.Quantity);
        Assert.Equal(product.Price * quantity, result.Value.TotalPrice);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenProductNotFound()
    {
        // Arrange
        var userId = 1;
        await TestDataHelper.CreateTestUser(DbContext, userId);
        await TestDataHelper.CreateTestCart(DbContext, userId);
        _mockCurrentUserService.UserId.Returns(userId.ToString());

        // Act
        var result = await _handler.Handle(
            new AddToCartCommand(999, 1), 
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(Errors.Product.NotFound, result.FirstError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenInsufficientStock()
    {
        // Arrange
        var userId = 1;
        await TestDataHelper.CreateTestUser(DbContext, userId);
        var (product, _) = await TestDataHelper.CreateTestProduct(DbContext, stockQuantity: 5);
        await TestDataHelper.CreateTestCart(DbContext, userId);
        _mockCurrentUserService.UserId.Returns(userId.ToString());

        // Act
        var result = await _handler.Handle(
            new AddToCartCommand(product.Id, 10), 
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Out of stock", result.FirstError.Description);
    }
}
