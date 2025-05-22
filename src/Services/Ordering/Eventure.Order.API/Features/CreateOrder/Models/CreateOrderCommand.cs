namespace Eventure.Order.API.Features.CreateOrder.Models;

public record CreateOrderCommand(Guid UserId, List<CreateOrderItemDto> Items);

public record CreateOrderItemDto(
    Guid EventId,
    string EventName,
    decimal UnitPrice,
    int Quantity
);
