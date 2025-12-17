using Carter;
using Eventure.Order.API.Features.GetAllOrdersForUser.Models;
using Eventure.Order.API.Infrastructure;
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
            [AsParameters] GetAllOrdersForUserRequest request,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<GetAllOrdersForUserResponse>(
                new GetAllOrdersForUserQuery(userId, request.Page, request.PageSize), ct);

            return TypedResults.Ok(result);
        })
        .WithName("GetAllOrdersForUser")
        .WithDescription("Returns the list of orders for a given user.")
        .AddEndpointFilter<ValidationFilter<GetAllOrdersForUserRequest>>()
        .Produces<GetAllOrdersForUserResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}

