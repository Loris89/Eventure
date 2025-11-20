namespace Eventure.Order.API.Features.GetAllOrdersForUser.Models;

public record GetAllOrdersForUserResponse(
    IEnumerable<OrderDto> Orders,
    int TotalCount,
    int CurrentPage,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);

public record OrderDto(Guid Id, Guid UserId, decimal Total, string Status, IEnumerable<OrderItemDto> OrderItems);

public record OrderItemDto(Guid Id, Guid EventId, string EventName, decimal UnitPrice, int Quantity, decimal TotalPrice);