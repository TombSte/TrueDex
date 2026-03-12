using PokeApiNet;
using TrueDex.Api.Extensions;
using TrueDex.Api.Implementations;
using TrueDex.Api.Interfaces;

namespace TrueDex.Api;

public static class DependencyInjections
{
    extension(IServiceCollection services)
    {
        public void AddClients()
        {
            services.AddScoped<PokeApiClient>();
        }

        public void AddServices()
        {
            services.AddRetryPolicies();
            services.AddScoped<IPokemonService, PokemonService>();
        }
    }
}