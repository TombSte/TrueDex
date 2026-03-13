using FunTranslationClient;

namespace TrueDex.Api.Strategies;

public interface ITranslationStrategy
{
    public Task<TranslationResponse> ExecuteAsync(ITranslationClient client, string inputText);
}