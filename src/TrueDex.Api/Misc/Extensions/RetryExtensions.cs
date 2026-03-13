using FluentResults;
using PokeApiNet;
using Polly;
using Polly.Registry;
using Polly.Retry;

namespace TrueDex.Api.Extensions;

public static class RetryExtensions
{
    private static class Pipelines
    {
        public const string DefaultRetryPolicy = "DefaultRetryPolicy";    
    }
    
    public static void AddRetryPolicies(this IServiceCollection services)
    {
        services.AddResiliencePipeline(Pipelines.DefaultRetryPolicy, builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>()
            });
            builder.AddTimeout(TimeSpan.FromSeconds(10));
        });
    }

    public static ResiliencePipeline GetDefaultRetryPolicy(this ResiliencePipelineProvider<string> pipelineProvider)
        => pipelineProvider.GetPipeline(Pipelines.DefaultRetryPolicy);
}