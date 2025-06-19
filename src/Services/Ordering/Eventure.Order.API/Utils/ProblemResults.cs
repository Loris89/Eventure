namespace Eventure.Order.API.Utils;

public static class ProblemResults
{
    public static IResult ToNotFoundProblem(string detail)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Not Found",
            detail: detail
        );
    }

    public static IResult ToBadRequestProblem(string detail)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            detail: detail
        );
    }

    public static IResult ToInternalProblem(string detail)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Internal Server Error",
            detail: detail
        );
    }
}
