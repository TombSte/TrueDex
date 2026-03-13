using FunTranslationClient;

namespace TrueDex.Api.Strategies;

public class ShakespeareTranslationStrategy : ITranslationStrategy
{
    public Task<TranslationResponse> ExecuteAsync(ITranslationClient client, string inputText)
    {
        return client.TranslateShakespeareAsync(new TranslationRequest()
        {
            Text = inputText
        });
    }
}