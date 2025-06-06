using Eventure.Order.API.Domain.Common;
using Eventure.Order.API.Infrastructure;
using System.Text.Json.Serialization;

namespace Eventure.Order.API.Domain.Orders;

public sealed class Order : Aggregate<Guid>
{
    [JsonInclude]
    public Guid UserId { get; private set; }

    [JsonInclude]
    public List<OrderItem> Items { get; private set; } = [];

    [JsonInclude]
    [JsonConverter(typeof(SmartEnumJsonConverter<OrderStatus>))]
    public OrderStatus Status { get; private set; }

    [JsonInclude]
    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);

    [JsonConstructor]
    private Order() { }

    private Order(Guid id, Guid userId, IEnumerable<OrderItem> items)
    {
        if (!items.Any())
            throw new InvalidOperationException("An order must have at least one item.");

        Id = id;
        UserId = userId;
        Items.AddRange(items);
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    public static Order Create(Guid userId, IEnumerable<OrderItem> items)
    {
        var id = Guid.NewGuid();
        return new Order(id, userId, items);
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Only orders in 'Created' state can be paid.");

        Status = OrderStatus.Paid;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Only orders that are not paid or already cancelled can be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}

