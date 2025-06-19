using Carter;
using Eventure.Order.API.Features.CancelOrder.Models;
using Eventure.Order.API.Features.GetOrderById.Models;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Eventure.Order.API.Features.CancelOrder;

public class CancelOrderEndpoint : CarterModule
{
    public CancelOrderEndpoint()
        : base("/api")
    {
        WithTags("Orders");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/orders/{id:guid}/cancel", async (
            Guid id,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<Result>(new CancelOrderCommand(id), ct);
            if (result.IsSuccess)
            {
                return Results.Ok();
            }

            return Results.NotFound(result.Errors[0].Message);
        })
        .WithSummary("Cancel an existing order")
        .WithDescription("Sets the status of an order to 'Cancelled' if allowed")
        .Produces<GetOrderByIdResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
