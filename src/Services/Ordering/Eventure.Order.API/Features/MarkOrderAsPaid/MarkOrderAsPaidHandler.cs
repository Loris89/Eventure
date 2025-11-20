using Eventure.Order.API.Exceptions;
using Eventure.Order.API.Features.MarkOrderAsPaid.Models;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.MarkOrderAsPaid;

public class MarkOrderAsPaidHandler
{
    public static async Task Handle(
        MarkOrderAsPaidCommand command,
        IDocumentStore store,
        ILogger<MarkOrderAsPaidHandler> logger,
        CancellationToken ct)
    {
        await using var session = store.DirtyTrackedSession(); // We need tracking (like EF...)

        var order = await session.LoadAsync<OrderAggregate>(command.Id, ct)
            ?? throw new NotFoundException($"Order {command.Id} not found");

        order.MarkAsPaid();
        await session.SaveChangesAsync(ct);

        logger.LogInformation("Order with ID {OrderId} marked as paid", order.Id);
    }
}
