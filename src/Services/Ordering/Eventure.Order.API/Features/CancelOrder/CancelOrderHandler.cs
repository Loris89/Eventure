using Eventure.Order.API.Features.CancelOrder.Models;
using FluentResults;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.CancelOrder;

public class CancelOrderHandler
{
    public static async Task<Result> Handle(
        CancelOrderCommand command, 
        IDocumentSession session,
        ILogger<CancelOrderHandler> logger,
        CancellationToken ct)
    {
        var order = await session.LoadAsync<OrderAggregate>(command.Id, ct);

        if (order is null)
        {
            logger.LogWarning("Order with ID {OrderId} not found", command.Id);
            return Result.Fail($"Order {command.Id} not found.");
        }

        order.Cancel();
        session.Store(order);
        await session.SaveChangesAsync(ct);

        logger.LogInformation("Order with ID {OrderId} cancelled", order.Id);
        return Result.Ok();
    }
}
