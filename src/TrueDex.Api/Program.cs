using FluentValidation;
using Scalar.AspNetCore;
using TrueDex.Api;
using TrueDex.Api.MinimalApi;
using TrueDex.Api.Misc.Behaviors;
using TrueDex.Api.Misc.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddClients();
builder.Services.AddServices();
builder.Services.AddStrategies();

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [typeof(Program)];
    options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
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

//app.UseHttpsRedirection();
app.UseCors();

app.AddTrueDexMinimalApi();

app.Run();
