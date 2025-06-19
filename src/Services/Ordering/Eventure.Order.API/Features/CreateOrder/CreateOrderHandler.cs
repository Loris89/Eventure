using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Features.CreateOrder.Models;
using FluentValidation;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.CreateOrder;

public class CreateOrderHandler
{
    public static async Task<Guid> Handle(
        CreateOrderCommand command, 
        IDocumentSession session,
        IValidator<CreateOrderCommand> validator,
        ILogger<CreateOrderHandler> logger, 
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for command {CreateOrderCommand}", command);
            throw new ValidationException(validationResult.Errors);
        }

        var items = command.Items.Select(i =>
            OrderItem.Create(i.EventId, i.EventName, i.UnitPrice, i.Quantity)).ToList();

        var order = OrderAggregate.Create(command.UserId, items);

        session.Store(order);
        await session.SaveChangesAsync(ct);

        logger.LogInformation("Order {OrderId} was created for user {UserId}", order.Id, order.UserId);

        return order.Id;
    }
}
