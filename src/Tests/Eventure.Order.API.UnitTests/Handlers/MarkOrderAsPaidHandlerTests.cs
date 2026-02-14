using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Exceptions;
using Eventure.Order.API.Features.MarkOrderAsPaid;
using Eventure.Order.API.Features.MarkOrderAsPaid.Models;
using Marten;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.UnitTests.Handlers;

public class MarkOrderAsPaidHandlerTests
{
    private readonly IDocumentStore _store;
    private readonly IDocumentSession _session;
    private readonly ILogger<MarkOrderAsPaidHandler> _logger;

    public MarkOrderAsPaidHandlerTests()
    {
        _store = Substitute.For<IDocumentStore>();
        _session = Substitute.For<IDocumentSession>();
        _logger = Substitute.For<ILogger<MarkOrderAsPaidHandler>>();

        // Setup: DirtyTrackedSession returns our mock session
        _store.DirtyTrackedSession().Returns(_session);
    }

    [Fact]
    public async Task Handle_WhenOrderExistsAndCanBeMarkedAsPaid_ShouldMarkAsPaidAndSave()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = CreateTestOrderWithStatus(orderId);

        _session.LoadAsync<OrderAggregate>(orderId, Arg.Any<CancellationToken>())
            .Returns(order);

        var command = new MarkOrderAsPaidCommand(orderId);

        // Act
        await MarkOrderAsPaidHandler.Handle(command, _store, _logger, CancellationToken.None);

        // Assert
        order.Status.ShouldBe(OrderStatus.Paid);
        await _session.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _session.LoadAsync<OrderAggregate>(orderId, Arg.Any<CancellationToken>())
            .Returns((OrderAggregate?)null);

        var command = new MarkOrderAsPaidCommand(orderId);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
            await MarkOrderAsPaidHandler.Handle(command, _store, _logger, CancellationToken.None)
        );

        // Verify SaveChanges was NOT called
        await _session.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("Cancelled")]
    [InlineData("Failed")]
    [InlineData("Paid")]
    public async Task Handle_WhenOrderIsInTerminalState_ShouldThrowDomainException(string statusName)
    {
        // Arrange
        OrderStatus terminalStatus = OrderStatus.FromName(statusName);

        var orderId = Guid.NewGuid();
        var order = CreateTestOrderWithStatus(orderId, terminalStatus);

        _session.LoadAsync<OrderAggregate>(orderId, Arg.Any<CancellationToken>())
            .Returns(order);

        var command = new MarkOrderAsPaidCommand(orderId);

        // Act & Assert
        await Should.ThrowAsync<DomainRuleViolationException>(async () =>
            await MarkOrderAsPaidHandler.Handle(command, _store, _logger, CancellationToken.None)
        );

        // Verify SaveChanges was NOT called
        await _session.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static OrderAggregate CreateTestOrderWithStatus(Guid orderId, OrderStatus? status = null)
    {
        var items = new[] { OrderItem.Create(Guid.NewGuid(), "Test Event", 10m, 1) };
        var order = OrderAggregate.Create(Guid.NewGuid(), items);

        if (status is not null)
        {
            if (status == OrderStatus.Cancelled)
                order.Cancel();
            else if (status == OrderStatus.Failed)
                order.MarkAsFailed();
            else if (status == OrderStatus.Paid)
                order.MarkAsPaid();
        }

        typeof(OrderAggregate)
            .GetProperty(nameof(OrderAggregate.Id))!
            .SetValue(order, orderId);

        return order;
    }
}