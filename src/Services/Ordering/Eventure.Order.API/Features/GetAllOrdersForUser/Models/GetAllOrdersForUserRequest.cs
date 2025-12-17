namespace Eventure.Order.API.Features.GetAllOrdersForUser.Models;

public sealed record GetAllOrdersForUserRequest(int Page = 1, int PageSize = 10);
