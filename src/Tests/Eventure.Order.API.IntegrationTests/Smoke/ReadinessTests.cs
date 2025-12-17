using Eventure.Order.API.IntegrationTests.Infrastructure;
using Shouldly;
using System.Net;

namespace Eventure.Order.API.IntegrationTests.Smoke;

[Collection(IntegrationTestCollection.Name)]
public sealed class ReadinessTests
{
    private readonly PostgresContainerFixture _db;

    public ReadinessTests(PostgresContainerFixture db)
    {
        _db = db;
    }

    [Fact]
    public async Task Ready_Should_Return_200_When_Db_Is_Reachable()
    {
        await using var factory = new OrderingApiFactory(_db.ConnectionString);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/ready");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
