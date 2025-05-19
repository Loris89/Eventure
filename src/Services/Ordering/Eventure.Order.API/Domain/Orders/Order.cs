using Eventure.Order.API.Domain.Common;

namespace Eventure.Order.API.Domain.Orders;

public sealed class Order : Aggregate<Guid>
{
    public Guid UserId { get; private set; }

    private readonly List<OrderItem> _items = [];

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public OrderStatus Status { get; private set; }

    private Order() { }

    private Order(Guid id, Guid userId, IEnumerable<OrderItem> items)
    {
        if (!items.Any())
            throw new InvalidOperationException("An order must have at least one item.");

        Id = id;
        UserId = userId;
        _items.AddRange(items);
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;

        // AddDomainEvent(new OrderCreatedDomainEvent(Id)); // in futuro
    }

    public static Order Create(Guid userId, IEnumerable<OrderItem> items)
    {
        var id = Guid.NewGuid();
        return new Order(id, userId, items);
    }

    public decimal TotalAmount => _items.Sum(i => i.TotalPrice);

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Only orders in 'Created' state can be paid.");

        Status = OrderStatus.Paid;

        // AddDomainEvent(new OrderPaidDomainEvent(Id)); // in futuro
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Only orders that are not paid or already cancelled can be cancelled.");

        Status = OrderStatus.Cancelled;

        // AddDomainEvent(new OrderCancelledDomainEvent(Id)); // in futuro
    }
}
