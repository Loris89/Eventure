using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Exceptions;
using Shouldly;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.UnitTests.Domain;

public class OrderTests
{
    #region Create

    [Fact]
    public void Create_WithValidItems_ShouldCreateOrderInCreatedStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var items = CreateValidOrderItems(quantity: 2);

        // Act
        var order = OrderAggregate.Create(userId, items);

        // Assert
        order.ShouldNotBeNull();
        order.Id.ShouldNotBe(Guid.Empty);
        order.UserId.ShouldBe(userId);
        order.Status.ShouldBe(OrderStatus.Created);
        order.Items.Count.ShouldBe(2);
        order.CreatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Create_WithEmptyItems_ShouldThrowDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyItems = Enumerable.Empty<OrderItem>();

        // Act & Assert
        var exception = Should.Throw<DomainRuleViolationException>(
            () => OrderAggregate.Create(userId, emptyItems)
        );

        exception.Message.ShouldContain("at least one item");
    }

    [Fact]
    public void Create_ShouldCalculateTotalAmountCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var items = new[]
        {
            OrderItem.Create(Guid.NewGuid(), "Event A", unitPrice: 10.00m, quantity: 2), // 20.00
            OrderItem.Create(Guid.NewGuid(), "Event B", unitPrice: 15.50m, quantity: 1), // 15.50
        };

        // Act
        var order = OrderAggregate.Create(userId, items);

        // Assert
        order.TotalAmount.ShouldBe(35.50m);
    }

    #endregion

    #region MarkAsPaid

    [Fact]
    public void MarkAsPaid_WhenStatusIsCreated_ShouldTransitionToPaid()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);

        // Act
        order.MarkAsPaid();

        // Assert
        order.Status.ShouldBe(OrderStatus.Paid);
        order.LastModified.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsPaid_WhenStatusIsPaid_ShouldThrowWithClearMessage()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);
        order.MarkAsPaid(); // Now it's Paid

        // Act & Assert
        var exception = Should.Throw<DomainRuleViolationException>(
            () => order.MarkAsPaid()
        );

        exception.Message.ShouldContain("already been paid");
    }

    [Fact]
    public void MarkAsPaid_WhenStatusIsCancelled_ShouldThrowWithClearMessage()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);
        order.Cancel(); // Now it's Cancelled

        // Act & Assert
        var exception = Should.Throw<DomainRuleViolationException>(
            () => order.MarkAsPaid()
        );

        exception.Message.ShouldContain("already been cancelled");
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_WhenStatusIsCreated_ShouldTransitionToCancelled()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);

        // Act
        order.Cancel();

        // Assert
        order.Status.ShouldBe(OrderStatus.Cancelled);
        order.LastModified.ShouldNotBeNull();
    }

    [Fact]
    public void Cancel_WhenStatusIsPaid_ShouldThrowWithClearMessage()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);
        order.MarkAsPaid(); // Now it's Paid

        // Act & Assert
        var exception = Should.Throw<DomainRuleViolationException>(
            () => order.Cancel()
        );

        exception.Message.ShouldContain("already been paid");
    }

    [Fact]
    public void Cancel_WhenStatusIsCancelled_ShouldThrowWithClearMessage()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);
        order.Cancel(); // Now it's Cancelled

        // Act & Assert
        var exception = Should.Throw<DomainRuleViolationException>(
            () => order.Cancel()
        );

        exception.Message.ShouldContain("already been cancelled");
    }

    #endregion

    #region MarkAsFailed

    [Fact]
    public void MarkAsFailed_WhenStatusIsCreated_ShouldTransitionToFailed()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);

        // Act
        order.MarkAsFailed();

        // Assert
        order.Status.ShouldBe(OrderStatus.Failed);
        order.LastModified.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsFailed_WhenStatusIsPaid_ShouldThrow()
    {
        // Arrange
        var order = CreateOrderInStatus(OrderStatus.Created);
        order.MarkAsPaid();

        // Act & Assert
        Should.Throw<DomainRuleViolationException>(
            () => order.MarkAsFailed()
        );
    }

    #endregion

    private static OrderAggregate CreateOrderInStatus(OrderStatus targetStatus)
    {
        var order = OrderAggregate.Create(
            Guid.NewGuid(),
            CreateValidOrderItems(quantity: 1)
        );

        if (targetStatus == OrderStatus.Created)
            return order;

        return order;
    }

    private static List<OrderItem> CreateValidOrderItems(int quantity = 1)
    {
        return Enumerable.Range(1, quantity)
            .Select(i => OrderItem.Create(
                eventId: Guid.NewGuid(),
                eventName: $"Test Event {i}",
                unitPrice: 10.00m * i,
                quantity: i
            ))
            .ToList();
    }
}
