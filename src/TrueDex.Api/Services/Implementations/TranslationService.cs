using FluentResults;
using FunTranslationClient;
using Polly.Registry;
using TrueDex.Api.Extensions;
using TrueDex.Api.Misc.Errors;
using TrueDex.Api.Services.Interfaces;
using TrueDex.Api.Strategies;

namespace TrueDex.Api.Services.Implementations;

public class TranslationService(
    ITranslationStrategySelector strategySelector, 
    ResiliencePipelineProvider<string> pipelineProvider, 
    ITranslationClient client,
    ILogger<TranslationService> logger) : ITranslationService
{
    private ITranslationStrategy _strategy = strategySelector.GetDefaultTranslationStrategy();
    public async ValueTask<Result<string>> TranslateAsync(string input)
    {
        string translatedText;

        try
        {
            translatedText = await pipelineProvider
                .GetDefaultRetryPolicy()
                .ExecuteAsync<string>(async ctx =>
                {
                    var response = await _strategy.ExecuteAsync(client, input);
                    translatedText = response.Contents.Translated;
                    return translatedText;
                });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting pokemon species");
            return ExternalErrors.TranslationError;
        }
        
        return translatedText;
    }

    public void SetTranslationStrategy(string habitat)
    {
        _strategy = strategySelector.GetTranslationStrategy(habitat);
    }
}