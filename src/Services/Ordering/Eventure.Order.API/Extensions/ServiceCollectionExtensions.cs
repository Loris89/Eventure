using Eventure.Order.API.Domain.Orders;
using Eventure.Order.API.Infrastructure;
using Marten;
using Marten.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Weasel.Core;
using OrderAggregate = Eventure.Order.API.Domain.Orders.Order;

namespace Eventure.Order.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        services.AddMarten(options =>
        {
            options.Connection(configuration.GetConnectionString("OrderingDb")!);
            options.DatabaseSchemaName = "ordering";
            options.Serializer(ConfigureJsonSerializer());
            options.Schema.For<OrderAggregate>().Identity(x => x.Id);
        });

        if (env.IsDevelopment())
        {
            services.ConfigureMarten(opts =>
            {
                opts.AutoCreateSchemaObjects = AutoCreate.All;
            });
        }

        return services;
    }

    private static SystemTextJsonSerializer ConfigureJsonSerializer()
    {
        var options = ConfigureJsonOptions();
        return new SystemTextJsonSerializer(options);
    }

    private static JsonSerializerOptions ConfigureJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new SmartEnumJsonConverter<OrderStatus>());

        return options;
    }
}
