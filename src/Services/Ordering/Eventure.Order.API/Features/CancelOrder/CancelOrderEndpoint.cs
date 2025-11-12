using Carter;
using Eventure.Order.API.Features.CancelOrder.Models;
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
            await bus.InvokeAsync(new CancelOrderCommand(id), ct);
            return TypedResults.Ok();
        })
        .WithName("Cancel an existing order")
        .WithDescription("Sets the status of an order to 'Cancelled' if allowed")
        .Produces(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
