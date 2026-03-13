using FluentResults;

namespace TrueDex.Api.Services.Interfaces;

public interface ITranslationService
{
    ValueTask<Result<string>> TranslateAsync(string input);
    void SetTranslationStrategy(string habitat);
}