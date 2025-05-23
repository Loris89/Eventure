using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Features.CreateOrder.Models;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.CreateOrder;

public class CreateOrderHandler
{
    public static async Task<Guid> Handle(
        CreateOrderCommand command, 
        IDocumentSession session, 
        ILogger<CreateOrderHandler> logger, 
        CancellationToken ct)
    {
        var items = command.Items.Select(i =>
            OrderItem.Create(i.EventId, i.EventName, i.UnitPrice, i.Quantity)).ToList();

        var order = OrderAggregate.Create(command.UserId, items);

        session.Store(order);
        await session.SaveChangesAsync(ct);

        logger.LogInformation("Ordine {OrderId} creato per utente {UserId}", order.Id, order.UserId);

        return order.Id;
    }
}
