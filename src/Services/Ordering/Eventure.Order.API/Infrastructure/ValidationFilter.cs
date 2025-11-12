using FluentValidation;

namespace Eventure.Order.API.Infrastructure;

public class ValidationFilter<T>(IValidator<T>? validator, ILogger<ValidationFilter<T>> logger) : IEndpointFilter where T : class
{
    private readonly IValidator<T>? _validator = validator;
    private readonly ILogger<ValidationFilter<T>> _logger = logger;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (_validator is null)
        {
            return await next(context);
        }

        var command = context.Arguments
            .OfType<T>()
            .FirstOrDefault();

        if (command is null)
        {
            return await next(context);
        }

        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(
                "Validation failed for {CommandType}. Errors: {@ValidationErrors}",
                typeof(T).Name,
                validationResult.Errors
            );

            return Results.ValidationProblem(
                validationResult.ToDictionary(),
                title: "Validation failed"
            );
        }

        return await next(context);
    }
}
