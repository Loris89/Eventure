using Eventure.Order.API.Features.GetAllOrdersForUser.Models;
using Eventure.Order.API.Utils.Pagination;
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

        PaginationMetadata paginationMetadata = PaginationHelper.Calculate(totalCount, query.Page, query.PageSize);

        logger.LogInformation(
            "Retrieved page {Page}/{TotalPages} with {Count} orders for user {UserId}",
            query.Page, paginationMetadata.TotalPages, orders.Count, query.UserId);

        return new GetAllOrdersForUserResponse(
            orderDtos,
            totalCount,
            query.Page,
            query.PageSize,
            paginationMetadata.TotalPages,
            paginationMetadata.HasNextPage,
            paginationMetadata.HasPreviousPage
        );
    }
}