using FluentResults;
using TrueDex.Api.Misc.Errors;

namespace TrueDex.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemResult(this ResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var error = result.Errors.FirstOrDefault();
        var (statusCode, title, detail, type) = MapError(error);

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            type: type,
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = result.Errors.Select(x => x.Message).ToArray()
            });
    }
    
    private static (int StatusCode, string Title, string Detail, string Type) MapError(IError? error)
        => error?.Message switch
        {
            //customize here error responses
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "The request could not be completed because of an internal error.",
                "https://httpstatuses.com/500")
        };
}
