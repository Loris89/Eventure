using Carter;
using Eventure.Order.API.Extensions;
using Eventure.Order.API.Features.CreateOrder;
using Eventure.Order.API.Handlers;
using FluentValidation;
using Scalar.AspNetCore;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Host.UseWolverine();

builder.Services.AddCarter();

builder.Services.AddMartenConfiguration(
    builder.Configuration,
    builder.Environment
);

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapCarter();
app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.Run();