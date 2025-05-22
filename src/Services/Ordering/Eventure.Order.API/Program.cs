using Carter;
using Eventure.Order.API.Extensions;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseWolverine();

builder.Services.AddCarter();

builder.Services.AddMartenConfiguration(
    builder.Configuration,
    builder.Environment
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapCarter();

app.UseHttpsRedirection();

app.Run();