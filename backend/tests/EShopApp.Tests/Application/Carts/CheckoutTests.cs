using ErrorOr;
using EShopApp.Application.Carts.Commands.Checkout;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Tests.Testing;
using Moq;

namespace EShopApp.Tests.Application.Carts;

public class CheckoutTests : TestBase
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly CheckoutCommandHandler _handler;

    public CheckoutTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new CheckoutCommandHandler(DbContext, _currentUserServiceMock.Object, _paymentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCart_ShouldCreatePaymentIntent()
    {
        // Arrange
        var user = await TestDataHelper.CreateTestUser(DbContext);
        var (product, _) = await TestDataHelper.CreateTestProduct(DbContext, price: 100m, stockQuantity: 10);
        var cart = await TestDataHelper.CreateTestCart(DbContext, user.Id);
        await TestDataHelper.CreateTestCartItem(DbContext, cart, product, quantity: 2);

        _currentUserServiceMock.Setup(x => x.UserId).Returns(user.Id.ToString());
        _paymentServiceMock.Setup(x => x.CreatePaymentIntentAsync(It.IsAny<PaymentIntentOptionsDto>()))
            .ReturnsAsync(new PaymentIntentResult("test_intent_id", "statues", "client_secret", 0, 0, "usd", "description", new Dictionary<string, string>()));

        // Act
        var result = await _handler.Handle(new CheckoutCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsError is false);
        Assert.Equal("test_intent_id", result.Value.PaymentIntentId);
        Assert.Equal(1, DbContext.Reservations.Count());
        Assert.Equal(1, DbContext.InventoryTransactions.Count());
    }

    [Fact]
    public async Task Handle_WithEmptyCart_ShouldReturnError()
    {
        // Arrange
        var user = await TestDataHelper.CreateTestUser(DbContext);
        var cart = await TestDataHelper.CreateTestCart(DbContext, user.Id);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(user.Id.ToString());

        // Act
        var result = await _handler.Handle(new CheckoutCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Conflict, result.FirstError.Type);
        Assert.Equal("Cannot checkout on an empty cart", result.FirstError.Description);
    }

    [Fact]
    public async Task Handle_WithInsufficientInventory_ShouldReturnError()
    {
        // Arrange
        var user = await TestDataHelper.CreateTestUser(DbContext);
        var (product, _) = await TestDataHelper.CreateTestProduct(DbContext, price: 100m, stockQuantity: 1);
        var cart = await TestDataHelper.CreateTestCart(DbContext, user.Id);
        await TestDataHelper.CreateTestCartItem(DbContext, cart, product, quantity: 2);

        _currentUserServiceMock.Setup(x => x.UserId).Returns(user.Id.ToString());

        // Act
        var result = await _handler.Handle(new CheckoutCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Conflict, result.FirstError.Type);
        Assert.Contains("Insufficient stock", result.FirstError.Description);
    }
}
