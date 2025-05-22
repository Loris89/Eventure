using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Features.CreateOrder.Models;
using FluentResults;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.CreateOrder;

public class CreateOrderHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateOrderCommand command, 
        IDocumentSession session, 
        ILogger<CreateOrderHandler> logger, 
        CancellationToken ct)
    {
        try
        {
            var items = command.Items.Select(i =>
                OrderItem.Create(i.EventId, i.EventName, i.UnitPrice, i.Quantity)).ToList();

            var order = OrderAggregate.Create(command.UserId, items);

            session.Store(order);
            await session.SaveChangesAsync(ct);

            return Result.Ok(order.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Errore durante la creazione dell'ordine");
            return Result.Fail("Errore interno durante la creazione dell'ordine.");
        }
    }
}
