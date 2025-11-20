using Carter;
using Eventure.Order.API.Features.MarkOrderAsPaid.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Eventure.Order.API.Features.MarkOrderAsPaid;

public class MarkOrderAsPaidEndpoint : CarterModule
{
    public MarkOrderAsPaidEndpoint()
        : base("/api")
    {
        WithTags("Orders");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/orders/{id:guid}", async (
            Guid id,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            await bus.InvokeAsync(new MarkOrderAsPaidCommand(id), ct);
            return TypedResults.Ok();
        })
        .WithName("MarkOrderAsPaid")
        .WithDescription("Marks an order as paid by updating its status.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
