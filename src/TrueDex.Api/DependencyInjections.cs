using FunTranslationsClient;
using PokeApiNet;
using TrueDex.Api.Extensions;
using TrueDex.Api.Services.Implementations;
using TrueDex.Api.Services.Interfaces;
using TrueDex.Api.Strategies;

namespace TrueDex.Api;

public static class DependencyInjections
{
    extension(IServiceCollection services)
    {
        public void AddClients()
        {
            services.AddScoped<PokeApiClient>();
            services.AddFunTranslationClient();
        }

        public void AddServices()
        {
            services.AddRetryPolicies();
            services.AddScoped<IPokemonService, PokemonService>();
            services.AddScoped<ITranslationService, TranslationService>();
        }

        public void AddStrategies()
        {
            services.AddScoped<ITranslationStrategySelector, TranslationStrategySelector>();
            services.AddKeyedScoped<ITranslationStrategy, YodaTranslationStrategy>(StrategyKeys.Yoda);
            services.AddKeyedScoped<ITranslationStrategy, ShakespeareTranslationStrategy>(StrategyKeys.Default);
        }
    }
}
