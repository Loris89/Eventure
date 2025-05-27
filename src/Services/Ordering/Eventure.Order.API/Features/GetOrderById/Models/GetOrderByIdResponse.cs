namespace Eventure.Order.API.Features.GetOrderById.Models;

public record GetOrderByIdResponse(Guid Id, Guid UserId, decimal Total, string Status);
