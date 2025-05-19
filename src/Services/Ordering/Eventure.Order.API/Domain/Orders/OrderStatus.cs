using Ardalis.SmartEnum;

namespace Eventure.Order.API.Domain.Orders;

public sealed class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus Created = new(nameof(Created), 0);
    public static readonly OrderStatus Paid = new(nameof(Paid), 1);
    public static readonly OrderStatus Cancelled = new(nameof(Cancelled), 2);
    public static readonly OrderStatus Failed = new(nameof(Failed), 3);

    private OrderStatus(string name, int value) : base(name, value)
    {
    }
}
