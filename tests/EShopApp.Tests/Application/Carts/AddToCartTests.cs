using EShopApp.Application.Carts.Commands.AddToCart;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using EShopApp.Tests.Testing;
using Moq;

namespace EShopApp.Tests.Application.Carts;

public class AddToCartTests : TestBase
{
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly AddToCartCommandHandler _handler;

    public AddToCartTests()
    {
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _handler = new AddToCartCommandHandler(_mockCurrentUserService.Object, DbContext);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenValidRequest()
    {
        // Arrange
        var userId = 1;
        var quantity = 2;
        
        var user = await TestDataHelper.CreateTestUser(DbContext);
        var (product, _) = await TestDataHelper.CreateTestProduct(DbContext, stockQuantity: 5);
        await TestDataHelper.CreateTestCart(DbContext, user.Id);
        
        _mockCurrentUserService.Setup(x => x.UserId).Returns(userId.ToString());

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
        var user = await TestDataHelper.CreateTestUser(DbContext);
        await TestDataHelper.CreateTestCart(DbContext, user.Id);
        _mockCurrentUserService.Setup(x => x.UserId).Returns(user.Id.ToString());

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
        var user = await TestDataHelper.CreateTestUser(DbContext);
        var (product, _) = await TestDataHelper.CreateTestProduct(DbContext, stockQuantity: 5);
        await TestDataHelper.CreateTestCart(DbContext, user.Id);
        _mockCurrentUserService.Setup(x => x.UserId).Returns(user.Id.ToString());

        // Act
        var result = await _handler.Handle(
            new AddToCartCommand(product.Id, 10), 
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Out of stock", result.FirstError.Description);
    }
}
