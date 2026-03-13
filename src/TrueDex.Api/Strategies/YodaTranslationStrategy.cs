using FunTranslationClient;

namespace TrueDex.Api.Strategies;

public class YodaTranslationStrategy : ITranslationStrategy
{
    public Task<TranslationResponse> ExecuteAsync(ITranslationClient client, string inputText)
    {
        return client.TranslateYodaAsync(new TranslationRequest()
        {
            Text = inputText
        });
    }
}