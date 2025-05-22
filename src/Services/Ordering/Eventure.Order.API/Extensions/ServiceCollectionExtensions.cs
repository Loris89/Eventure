using Marten;
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
}
