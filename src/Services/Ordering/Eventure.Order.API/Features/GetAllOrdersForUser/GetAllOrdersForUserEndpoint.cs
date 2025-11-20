using Carter;
using Eventure.Order.API.Features.GetAllOrdersForUser.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Eventure.Order.API.Features.GetAllOrdersForUser;

public class GetAllOrdersForUserEndpoint : CarterModule
{
    public GetAllOrdersForUserEndpoint()
        : base("/api")
    {
        WithTags("Orders");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/user/{userId:guid}", async (
            Guid userId,
            int page,
            int pageSize,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            GetAllOrdersForUserResponse result = await bus.InvokeAsync<GetAllOrdersForUserResponse>(
                new GetAllOrdersForUserQuery(userId, page, pageSize), ct);
            return TypedResults.Ok(result);
        })
        .WithName("GetAllOrdersForUser")
        .WithDescription("Returns the list of orders for a given user.")
        .Produces<GetAllOrdersForUserResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}

