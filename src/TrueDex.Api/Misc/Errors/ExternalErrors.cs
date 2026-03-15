using FluentResults;

namespace TrueDex.Api.Misc.Errors;

public static class ExternalErrors
{
    public const string PokeApiErrorMessage = "POKE_API_ERROR";
    public const string PokeApiNotFoundErrorMessage = "POKE_API_NOTFOUND_ERROR";
    public const string TranslationErrorMessage = "TRANSLATION_ERROR";

    public static readonly Error PokeApiError = new(PokeApiErrorMessage);
    public static readonly Error PokeApiNotFoundError = new(PokeApiNotFoundErrorMessage);
    public static readonly Error TranslationError = new(TranslationErrorMessage);
}
