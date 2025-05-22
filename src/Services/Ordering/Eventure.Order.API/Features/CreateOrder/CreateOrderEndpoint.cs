using Carter;
using Eventure.Order.API.Features.CreateOrder.Models;
using FluentResults;
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
            var result = await bus.InvokeAsync<Result<Guid>>(ct);
            return result.IsSuccess
                ? Results.Created($"/orders/{result.Value}", new CreateOrderResponse(result.Value))
                : Results.Problem(result.Errors.First().Message);
        });
    }
}
