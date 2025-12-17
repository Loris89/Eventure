using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eventure.Order.API.IntegrationTests.Infrastructure;

public sealed class OrderingApiFactory : WebApplicationFactory<Program> // Costruisce il "Program"
{
    private readonly string _connectionString;

    public OrderingApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Postgresql must use a different connection string while executing tests
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:OrderingDb"] = _connectionString
            };

            config.AddInMemoryCollection(overrides);
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }
}
