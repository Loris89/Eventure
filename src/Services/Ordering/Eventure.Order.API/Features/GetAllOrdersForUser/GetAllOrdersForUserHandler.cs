using Eventure.Order.API.Features.GetAllOrdersForUser.Models;
using Marten;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Features.GetAllOrdersForUser;

public class GetAllOrdersForUserHandler
{
    public static async Task<GetAllOrdersForUserResponse> Handle(
        GetAllOrdersForUserQuery query, 
        IQuerySession session, 
        ILogger<GetAllOrdersForUserHandler> logger,
        CancellationToken ct)
    {
        var totalCount = await session.Query<OrderAggregate>()
            .Where(o => o.UserId == query.UserId)
            .CountAsync(ct);

        var orders = await session.Query<OrderAggregate>()
            .Where(o => o.UserId == query.UserId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var orderDtos = orders.Select(o => new OrderDto(
            o.Id,
            o.UserId,
            o.TotalAmount,
            o.Status.Name,
            o.Items.Select(oi => new OrderItemDto(
                oi.Id, oi.EventId, oi.EventName, oi.UnitPrice, oi.Quantity, oi.TotalPrice))
        )).ToList();

        int totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        bool hasNextPage = query.Page < totalPages;
        bool hasPreviousPage = query.Page > 1;

        logger.LogInformation(
            "Retrieved page {Page}/{TotalPages} with {Count} orders for user {UserId}",
            query.Page, totalPages, orders.Count, query.UserId);

        return new GetAllOrdersForUserResponse(
            orderDtos,
            totalCount,
            query.Page,
            query.PageSize,
            totalPages,
            hasNextPage,
            hasPreviousPage
        );
    }
}
