using Carter;
using Eventure.Order.API.Features.CreateOrder.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Eventure.Order.API.Features.CreateOrder;

public class CreateOrderEndpoint : CarterModule
{
    public CreateOrderEndpoint()
        : base("/api")
    {
        WithTags("Orders");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (
            CreateOrderCommand command,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            var orderId = await bus.InvokeAsync<Guid>(command, ct);
            return Results.Created($"/orders/{orderId}", new CreateOrderResponse(orderId));
        })
        .WithName("CreateOrder")
        .WithDescription("Creates a new order with one or more event tickets.")
        .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
