using Eventure.Order.API.Exceptions;
using Eventure.Order.API.Features.GetOrderById.Models;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.GetOrderById;

public class GetOrderByIdHandler
{
    public static async Task<GetOrderByIdResponse> Handle(
        GetOrderByIdQuery query, 
        IQuerySession session, 
        ILogger<GetOrderByIdResponse> logger,
        CancellationToken ct)
    {
        var order = await session.LoadAsync<OrderAggregate>(query.Id, ct)
            ?? throw new NotFoundException($"Order {query.Id} not found");

        logger.LogInformation("Order with id {OrderId} was found.", query.Id);

        return new GetOrderByIdResponse(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.Status.Name
        );
    }
}
