using Eventure.Order.API.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Eventure.Order.API.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class GetAllOrdersForUserTests
{
    private readonly PostgresContainerFixture _db;

    public GetAllOrdersForUserTests(PostgresContainerFixture db)
    {
        _db = db;
    }

    [Fact]
    public async Task Should_Return_Paged_Orders_For_User_With_Items()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var userId = Guid.NewGuid();

        // Seed: 3 ordini per lo stesso utente
        var orderIds = await Task.WhenAll(
            CreateOrder(client, userId, "Event A"),
            CreateOrder(client, userId, "Event B"),
            CreateOrder(client, userId, "Event C")
        );

        // Page 1, size 2
        var url = $"/api/orders/user/{userId}?page=1&pageSize=2";
        var response = await client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<GetAllOrdersForUserResponse>();
        body.ShouldNotBeNull();

        body!.TotalCount.ShouldBe(3);
        body.PageSize.ShouldBe(2);
        body.CurrentPage.ShouldBe(1);
        body.TotalPages.ShouldBe(2);
        body.HasPreviousPage.ShouldBeFalse();
        body.HasNextPage.ShouldBeTrue();

        body.Orders.Length.ShouldBe(2);

        foreach (var o in body.Orders)
        {
            o.UserId.ShouldBe(userId);
            o.Status.ShouldNotBeNullOrWhiteSpace();
            o.OrderItems.ShouldNotBeNull();
            o.OrderItems.Any().ShouldBeTrue();
        }

        // Page 2, size 2
        var url2 = $"/api/orders/user/{userId}?page=2&pageSize=2";
        var response2 = await client.GetAsync(url2);

        response2.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body2 = await response2.Content.ReadFromJsonAsync<GetAllOrdersForUserResponse>();
        body2.ShouldNotBeNull();

        body2!.Orders.Length.ShouldBe(1);
        body2.HasPreviousPage.ShouldBeTrue();
        body2.HasNextPage.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Return_400_When_Page_Is_Zero()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var userId = Guid.NewGuid();
        var response = await client.GetAsync($"/api/orders/user/{userId}?page=0&pageSize=10");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");

        var json = await response.Content.ReadAsStringAsync();
        var vpd = JsonSerializer.Deserialize<ValidationProblemDetails>(json);

        vpd.ShouldNotBeNull();
        vpd!.Errors.Keys
            .Any(k => k.Contains("Page", StringComparison.OrdinalIgnoreCase))
            .ShouldBeTrue();
    }

    private static async Task<Guid> CreateOrder(HttpClient client, Guid userId, string eventName)
    {
        var payload = new
        {
            userId,
            items = new[]
            {
                new
                {
                    eventId = Guid.NewGuid(),
                    eventName,
                    unitPrice = 10m,
                    quantity = 1
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/orders", payload);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        body.ShouldNotBeNull();
        body!.OrderId.ShouldNotBe(Guid.Empty);

        return body.OrderId;
    }

    private sealed record CreateOrderResponse(Guid OrderId);

    private sealed record GetAllOrdersForUserResponse(
        OrderDto[] Orders,
        int TotalCount,
        int CurrentPage,
        int PageSize,
        int TotalPages,
        bool HasNextPage,
        bool HasPreviousPage
    );

    private sealed record OrderDto(Guid Id, Guid UserId, decimal Total, string Status, OrderItemDto[] OrderItems);

    private sealed record OrderItemDto(Guid Id, Guid EventId, string EventName, decimal UnitPrice, int Quantity, decimal TotalPrice);
}