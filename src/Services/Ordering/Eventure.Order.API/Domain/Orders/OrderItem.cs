namespace Eventure.Order.API.Domain.Orders;

public sealed class OrderItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid EventId { get; private set; }
    public string EventName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

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

    public decimal TotalPrice => UnitPrice * Quantity;
}
