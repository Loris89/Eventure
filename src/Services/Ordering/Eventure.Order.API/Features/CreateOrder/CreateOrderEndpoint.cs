using Carter;
using Eventure.Order.API.Features.CreateOrder.Models;
using FluentResults;
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
        app.MapPost("/orders", async (CreateOrderCommand command, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<Result<Guid>>(command, ct);
            return result.IsSuccess
                ? Results.Created($"/orders/{result.Value}", new CreateOrderResponse(result.Value))
                : Results.Problem(result.Errors.First().Message);
        })
        .WithName("CreateOrder")
        .WithDescription("Crea un nuovo ordine con uno o più biglietti per eventi.")
        .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
