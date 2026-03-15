using System.Text.Json;
using AwesomeAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TrueDex.Api.Misc.Errors;
using TrueDex.Api.Misc.Extensions;

namespace TrueDex.Api.Test.Misc.Extensions;

public class ResultExtensionsTest
{
    [Fact]
    public async Task ToProblemResult_ShouldReturnProblemDetailsPayload_WhenResultHasErrors()
    {
        var result = Result.Fail(new Error("first-error")).WithError("second-error");

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.RequestServices = services.BuildServiceProvider();

        var problemResult = result.ToProblemResult();

        await problemResult.ExecuteAsync(httpContext);

        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        httpContext.Response.ContentType.Should().Be("application/problem+json");

        httpContext.Response.Body.Position = 0;
        using var responseJson = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var root = responseJson.RootElement;

        root.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status500InternalServerError);
        root.GetProperty("title").GetString().Should().Be("Unexpected error");
        root.GetProperty("detail").GetString().Should().Be("The request could not be completed because of an internal error.");

        var errors = root.GetProperty("errors").EnumerateArray().Select(static x => x.GetString()).ToArray();
        errors.Should().BeEquivalentTo(["first-error", "second-error"]);
    }

    [Fact]
    public async Task ToProblemResult_ShouldReturnNotFoundProblemDetails_WhenResultContainsPokemonNotFoundError()
    {
        var result = Result.Fail(ExternalErrors.PokeApiNotFoundError);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.RequestServices = services.BuildServiceProvider();

        var problemResult = result.ToProblemResult();

        await problemResult.ExecuteAsync(httpContext);

        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        httpContext.Response.ContentType.Should().Be("application/problem+json");

        httpContext.Response.Body.Position = 0;
        using var responseJson = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var root = responseJson.RootElement;

        root.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status404NotFound);
        root.GetProperty("title").GetString().Should().Be("Not found");
        root.GetProperty("detail").GetString().Should().Be("Pokemon does not exists.");

        var errors = root.GetProperty("errors").EnumerateArray().Select(static x => x.GetString()).ToArray();
        errors.Should().BeEquivalentTo([ExternalErrors.PokeApiNotFoundErrorMessage]);
    }

    [Fact]
    public void ToProblemResult_ShouldThrowArgumentNullException_WhenResultIsNull()
    {
        ResultBase? result = null;

        var action = () => result!.ToProblemResult();

        action.Should().Throw<ArgumentNullException>().WithParameterName("result");
    }
}
