using Eventure.Order.API.Features.CancelOrder.Models;
using FluentResults;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.CancelOrder;

public class CancelOrderHandler
{
    public static async Task<Result> Handle(
        CancelOrderCommand command, 
        IDocumentStore store,
        ILogger<CancelOrderHandler> logger,
        CancellationToken ct)
    {
        await using var session = store.DirtyTrackedSession(); // We need tracking (like EF...)
        var order = await session.LoadAsync<OrderAggregate>(command.Id, ct);

        if (order is null)
        {
            logger.LogWarning("Order with ID {OrderId} not found", command.Id);
            return Result.Fail($"Order {command.Id} not found.");
        }

        order.Cancel();
        await session.SaveChangesAsync(ct);

        logger.LogInformation("Order with ID {OrderId} cancelled", order.Id);
        return Result.Ok();
    }
}
