using Carter;
using Eventure.Order.API.Features.GetOrderById.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Eventure.Order.API.Features.GetOrderById;

public class GetOrderByIdEndpoint : CarterModule
{
    public GetOrderByIdEndpoint()
        : base("/api")
    {
        WithTags("Orders");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id:guid}", async (
            Guid id,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            var query = new GetOrderByIdQuery(id);
            var result = await bus.InvokeAsync<GetOrderByIdResponse>(query, ct);
            return Results.Ok(result);
        })
        .WithName("GetOrderById")
        .WithDescription("Returns details of a specific order by its unique identifier.")
        .Produces<GetOrderByIdResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
