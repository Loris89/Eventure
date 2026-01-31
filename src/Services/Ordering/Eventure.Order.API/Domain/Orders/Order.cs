using Eventure.Order.API.Domain.Common;
using Eventure.Order.API.Exceptions;
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
            throw new DomainRuleViolationException("An order must have at least one item.");

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
        EnsureCanTransitionFrom(OrderStatus.Created);

        Status = OrderStatus.Paid;
        LastModified = DateTime.UtcNow;
    }

    public void Cancel()
    {
        EnsureCanTransitionFrom(OrderStatus.Created);

        Status = OrderStatus.Cancelled;
        LastModified = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        EnsureCanTransitionFrom(OrderStatus.Created);

        Status = OrderStatus.Failed;
        LastModified = DateTime.UtcNow;
    }

    private void EnsureCanTransitionFrom(OrderStatus expectedCurrentStatus)
    {
        if (Status == expectedCurrentStatus)
            return;

        var reason = Status switch
        {
            _ when Status == OrderStatus.Paid => "order has already been paid",
            _ when Status == OrderStatus.Cancelled => "order has already been cancelled",
            _ when Status == OrderStatus.Failed => "order has already failed",
            _ => $"current state is '{Status.Name}'"
        };

        throw new DomainRuleViolationException(
            $"Invalid state transition. Cannot change order from '{Status.Name}' to '{expectedCurrentStatus.Name}': {reason}.");
    }
}