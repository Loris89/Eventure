namespace Eventure.Order.API.Features.GetAllOrdersForUser.Models;

public record GetAllOrdersForUserQuery(Guid UserId, int Page = 1, int PageSize = 10);