using Eventure.Order.API.Exceptions;
using Eventure.Order.API.Features.CancelOrder.Models;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.CancelOrder;

public class CancelOrderHandler
{
    public static async Task Handle(
        CancelOrderCommand command, 
        IDocumentStore store,
        ILogger<CancelOrderHandler> logger,
        CancellationToken ct)
    {
        await using var session = store.DirtyTrackedSession(); // We need tracking (like EF...)

        var order = await session.LoadAsync<OrderAggregate>(command.Id, ct) 
            ?? throw new NotFoundException($"Order {command.Id} not found");
        
        order.Cancel();
        await session.SaveChangesAsync(ct);

        logger.LogInformation("Order with ID {OrderId} cancelled", order.Id);
    }
}
