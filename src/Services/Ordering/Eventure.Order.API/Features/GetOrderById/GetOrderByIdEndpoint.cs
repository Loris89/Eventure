using Carter;
using Eventure.Order.API.Features.GetOrderById.Models;
using FluentResults;
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
            var result = await bus.InvokeAsync<Result<GetOrderByIdResponse>>(new GetOrderByIdQuery(id), ct);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.NotFound(result.Errors[0].Message);
        })
        .WithName("GetOrderById")
        .WithDescription("Returns details of a specific order by its unique identifier.")
        .Produces<GetOrderByIdResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
