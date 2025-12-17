using Eventure.Order.API.IntegrationTests.Infrastructure;
using Shouldly;
using System.Net;

namespace Eventure.Order.API.IntegrationTests.Smoke;

[Collection(IntegrationTestCollection.Name)]
public sealed class LivenessTests
{
    private readonly PostgresContainerFixture _db;

    public LivenessTests(PostgresContainerFixture db)
    {
        _db = db;
    }

    [Fact]
    public async Task Should_Bootstrap_Api()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/live");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
