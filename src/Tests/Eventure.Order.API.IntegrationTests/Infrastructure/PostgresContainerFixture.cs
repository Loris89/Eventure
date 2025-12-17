using Testcontainers.PostgreSql;

namespace Eventure.Order.API.IntegrationTests.Infrastructure;

public class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("ordering_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public Task InitializeAsync() => _container.StartAsync(); // Called BEFORE tests start

    public Task DisposeAsync() => _container.DisposeAsync().AsTask(); // Called at the END
}
