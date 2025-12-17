using Eventure.Order.API.IntegrationTests.Infrastructure;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateOrderTests
{
    private readonly PostgresContainerFixture _db;

    public CreateOrderTests(PostgresContainerFixture db)
    {
        _db = db;
    }

    [Fact]
    public async Task Create_Order_Should_Persist_Order()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var payload = new
        {
            userId = Guid.NewGuid(),
            items = new[]
            {
                new
                {
                    eventId = Guid.NewGuid(),
                    eventName = "Test Event",
                    unitPrice = 12.50m,
                    quantity = 2
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/orders", payload);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        body.ShouldNotBeNull();
        body!.OrderId.ShouldNotBe(Guid.Empty);

        var store = factory.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.LightweightSession();

        OrderAggregate? order = await session.LoadAsync<OrderAggregate>(body.OrderId);

        order.ShouldNotBeNull();
        order!.UserId.ShouldBe(payload.userId);
        order.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Create_Order_Should_Return_400_When_Items_Is_Empty()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var payload = new
        {
            userId = Guid.NewGuid(),
            items = Array.Empty<object>()
        };

        var response = await client.PostAsJsonAsync("/api/orders", payload);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");

        var problem = await ReadProblemAsync(response);
        problem.Status.ShouldBe(400);

        problem.ShouldBeOfType<ValidationProblemDetails>();

        var vpd = (ValidationProblemDetails)problem;
        vpd.Errors.ShouldContainKey("Items");
        vpd.Errors["Items"].Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Create_Order_Should_Return_400_When_Quantity_Is_Zero()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var payload = new
        {
            userId = Guid.NewGuid(),
            items = new[]
            {
                new
                {
                    eventId = Guid.NewGuid(),
                    eventName = "Test Event",
                    unitPrice = 10m,
                    quantity = 0
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/orders", payload);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");

        var problem = await ReadProblemAsync(response);
        problem.Status.ShouldBe(400);
        problem.ShouldBeOfType<ValidationProblemDetails>();

        var vpd = (ValidationProblemDetails)problem;

        vpd.Errors.Keys.Any(k => k.Contains("Quantity", StringComparison.OrdinalIgnoreCase))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task Create_Order_Should_Not_Persist_Anything_When_Request_Is_Invalid()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var userId = Guid.NewGuid();

        var invalidPayload = new
        {
            userId,
            items = Array.Empty<object>() // invalido: NotEmpty
        };

        var response = await client.PostAsJsonAsync("/api/orders", invalidPayload);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var store = factory.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var ordersForUser = await session.Query<OrderAggregate>()
            .Where(o => o.UserId == userId)
            .ToListAsync();

        ordersForUser.Count.ShouldBe(0);
    }

    private static async Task<ProblemDetails> ReadProblemAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        var vpd = JsonSerializer.Deserialize<ValidationProblemDetails>(json);
        if (vpd?.Errors is not null && vpd.Errors.Count > 0) return vpd;

        var pd = JsonSerializer.Deserialize<ProblemDetails>(json);
        pd.ShouldNotBeNull();
        return pd!;
    }

    private sealed record CreateOrderResponse(Guid OrderId);
}
