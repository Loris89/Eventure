using Carter;
using Eventure.Order.API.Extensions;
using Scalar.AspNetCore;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Host.UseWolverine();

builder.Services.AddCarter();

builder.Services.AddMartenConfiguration(
    builder.Configuration,
    builder.Environment
);

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapCarter();

app.UseHttpsRedirection();

app.Run();