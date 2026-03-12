using Scalar.AspNetCore;
using TrueDex.Api;
using TrueDex.Api.Mapping;
using TrueDex.Api.MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddClients();
builder.Services.AddServices();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfile<ResponseMappingProfile>();
});

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [typeof(Program)];
    options.PipelineBehaviors = [];
    options.StreamPipelineBehaviors = [];
});

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(pattern: "api/document.json");
    app.MapScalarApiReference("/docs", options =>
    {
        options.OpenApiRoutePattern = "api/document.json";
        options.Title = "TrueDex API - Your new super cool Pokedex";
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.AddTrueDexMinimalApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}