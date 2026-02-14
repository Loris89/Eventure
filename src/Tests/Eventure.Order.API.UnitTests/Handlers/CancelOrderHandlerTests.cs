using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Exceptions;
using Eventure.Order.API.Features.CancelOrder;
using Eventure.Order.API.Features.CancelOrder.Models;
using Marten;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.UnitTests.Handlers;

public class CancelOrderHandlerTests
{
    private readonly IDocumentStore _store;
    private readonly IDocumentSession _session;
    private readonly ILogger<CancelOrderHandler> _logger;

    public CancelOrderHandlerTests()
    {
        _store = Substitute.For<IDocumentStore>();
        _session = Substitute.For<IDocumentSession>();
        _logger = Substitute.For<ILogger<CancelOrderHandler>>();

        // Setup: DirtyTrackedSession returns our mock session
        _store.DirtyTrackedSession().Returns(_session);
    }

    [Fact]
    public async Task Handle_WhenOrderExistsAndCanBeCancelled_ShouldCancelAndSave()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId, OrderStatus.Created);

        _session.LoadAsync<OrderAggregate>(orderId, Arg.Any<CancellationToken>())
            .Returns(order);

        var command = new CancelOrderCommand(orderId);

        // Act
        await CancelOrderHandler.Handle(command, _store, _logger, CancellationToken.None);

        // Assert
        order.Status.ShouldBe(OrderStatus.Cancelled);
        await _session.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _session.LoadAsync<OrderAggregate>(orderId, Arg.Any<CancellationToken>())
            .Returns((OrderAggregate?)null);

        var command = new CancelOrderCommand(orderId);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
            await CancelOrderHandler.Handle(command, _store, _logger, CancellationToken.None)
        );

        // Verify SaveChanges was NOT called
        await _session.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderIsPaid_ShouldThrowDomainException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder(orderId, OrderStatus.Created);
        order.MarkAsPaid(); // Now it's Paid

        _session.LoadAsync<OrderAggregate>(orderId, Arg.Any<CancellationToken>())
            .Returns(order);

        var command = new CancelOrderCommand(orderId);

        // Act & Assert
        await Should.ThrowAsync<DomainRuleViolationException>(async () =>
            await CancelOrderHandler.Handle(command, _store, _logger, CancellationToken.None)
        );

        // Verify SaveChanges was NOT called (rollback behavior)
        await _session.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static OrderAggregate CreateTestOrder(Guid orderId, OrderStatus _)
    {
        var items = new[] { OrderItem.Create(Guid.NewGuid(), "Test Event", 10m, 1) };
        var order = OrderAggregate.Create(Guid.NewGuid(), items);

        typeof(OrderAggregate)
            .GetProperty(nameof(OrderAggregate.Id))!
            .SetValue(order, orderId);

        return order;
    }
}
