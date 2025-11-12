using Eventure.Order.API.Exceptions;
using Eventure.Order.API.Features.GetOrderById.Models;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.GetOrderById;

public class GetOrderByIdHandler
{
    public static async Task<GetOrderByIdResponse> Handle(GetOrderByIdQuery query, IQuerySession session)
    {
        var order = await session.LoadAsync<OrderAggregate>(query.Id)
            ?? throw new NotFoundException($"Order {query.Id} not found");

        return new GetOrderByIdResponse(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.Status.Name
        );
    }
}
