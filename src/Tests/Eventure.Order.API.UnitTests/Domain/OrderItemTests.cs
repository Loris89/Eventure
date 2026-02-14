using Eventure.Order.API.Domain.Orders;
using Shouldly;

namespace Eventure.Order.API.UnitTests.Domain;

public class OrderItemTests
{
    #region Create - Happy Path

    [Fact]
    public void Create_WithValidParameters_ShouldCreateOrderItem()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        const string eventName = "Concert ABC";
        const decimal unitPrice = 25.50m;
        const int quantity = 3;

        // Act
        var item = OrderItem.Create(eventId, eventName, unitPrice, quantity);

        // Assert
        item.ShouldNotBeNull();
        item.Id.ShouldNotBe(Guid.Empty);
        item.EventId.ShouldBe(eventId);
        item.EventName.ShouldBe(eventName);
        item.UnitPrice.ShouldBe(unitPrice);
        item.Quantity.ShouldBe(quantity);
    }

    [Fact]
    public void Create_ShouldCalculateTotalPriceCorrectly()
    {
        // Arrange & Act
        var item = OrderItem.Create(
            Guid.NewGuid(),
            "Event",
            unitPrice: 12.50m,
            quantity: 4
        );

        // Assert
        item.TotalPrice.ShouldBe(50.00m); // 12.50 * 4
    }

    [Fact]
    public void Create_WithZeroUnitPrice_ShouldAllowFreeTickets()
    {
        // Arrange & Act - Free events should be allowed!
        var item = OrderItem.Create(
            Guid.NewGuid(),
            "Free Community Event",
            unitPrice: 0m,
            quantity: 2
        );

        // Assert
        item.UnitPrice.ShouldBe(0m);
        item.TotalPrice.ShouldBe(0m);
    }

    #endregion

    #region Create - Validation Failures

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithInvalidQuantity_ShouldThrow(int invalidQuantity)
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            OrderItem.Create(
                Guid.NewGuid(),
                "Event",
                unitPrice: 10m,
                quantity: invalidQuantity
            )
        );
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithNegativeUnitPrice_ShouldThrow(decimal negativePrice)
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() =>
            OrderItem.Create(
                Guid.NewGuid(),
                "Event",
                unitPrice: negativePrice,
                quantity: 1
            )
        );
    }

    #endregion

    #region TotalPrice Calculation Edge Cases

    [Fact]
    public void TotalPrice_WithLargeQuantity_ShouldNotOverflow()
    {
        // Arrange - Test con numeri grandi ma realistici
        var item = OrderItem.Create(
            Guid.NewGuid(),
            "Event",
            unitPrice: 999.99m,
            quantity: 1000
        );

        // Assert
        item.TotalPrice.ShouldBe(999_990.00m);
    }

    [Fact]
    public void TotalPrice_ShouldHandleDecimalPrecision()
    {
        // Arrange - Prezzi con centesimi
        var item = OrderItem.Create(
            Guid.NewGuid(),
            "Event",
            unitPrice: 33.33m,
            quantity: 3
        );

        // Assert
        item.TotalPrice.ShouldBe(99.99m);
    }

    #endregion
}
