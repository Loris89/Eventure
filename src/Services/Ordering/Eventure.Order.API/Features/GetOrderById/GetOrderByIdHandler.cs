using Eventure.Order.API.Features.GetOrderById.Models;
using FluentResults;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.GetOrderById;

public class GetOrderByIdHandler
{
    public static async Task<Result<GetOrderByIdResponse>> Handle(
        GetOrderByIdQuery query, 
        IQuerySession session,
        ILogger<GetOrderByIdHandler> logger)
    {
        var order = await session.LoadAsync<OrderAggregate>(query.Id);

        if (order is null)
        {
            logger.LogWarning("Order with ID {OrderId} not found", query.Id);
            return Result.Fail<GetOrderByIdResponse>($"L'ordine {query.Id} non è stato trovato.");
        }

        return new GetOrderByIdResponse(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.Status.Name
        );
    }
}
