using Carter;
using Eventure.Order.API.Extensions;
using Eventure.Order.API.Features.CreateOrder;
using Eventure.Order.API.Handlers;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Host.UseWolverine();

// "Carter" makes the minimal endpoints registration much easier.
// This is the only line of code we need.
builder.Services.AddCarter();

builder.Services.AddMartenConfiguration(
    builder.Configuration,
    builder.Environment
);

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddNpgSql(
        builder.Configuration.GetConnectionString("OrderingDb")!,
        name: "postgres",
        tags: ["ready"]);

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapCarter();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.Run();

// Expose the Minimal API entry point to the test project so WebApplicationFactory<Program>
// can locate and bootstrap the application for integration tests (top-level statements have no explicit Program type).
public partial class Program { }