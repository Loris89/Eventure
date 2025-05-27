using System.Text.Json.Serialization;

namespace Eventure.Order.API.Domain.Orders;

public sealed class OrderItem
{
    [JsonInclude]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [JsonInclude]
    public Guid EventId { get; private set; }

    [JsonInclude]
    public string EventName { get; private set; } = default!;

    [JsonInclude]
    public decimal UnitPrice { get; private set; }

    [JsonInclude]
    public int Quantity { get; private set; }

    [JsonInclude]
    public decimal TotalPrice => UnitPrice * Quantity;

    [JsonConstructor]
    private OrderItem() { }

    private OrderItem(Guid eventId, string eventName, decimal unitPrice, int quantity)
    {
        EventId = eventId;
        EventName = eventName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static OrderItem Create(Guid eventId, string eventName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));

        return new OrderItem(eventId, eventName, unitPrice, quantity);
    }
}
