using AwesomeAssertions;
using FunTranslationClient;
using Microsoft.Extensions.DependencyInjection;
using PokeApiNet;
using TrueDex.Api.Services.Implementations;
using TrueDex.Api.Services.Interfaces;

namespace TrueDex.Api.Test;

public class DependencyInjectionTests
{
    private IServiceProvider _sp;

    public DependencyInjectionTests()
    {
        var services = new ServiceCollection();
        
        #region Application dependencies
        services.AddLogging();
        #endregion
        
        services.AddClients();
        services.AddServices();
        services.AddStrategies();
        
        _sp = services.BuildServiceProvider();
    }

    [Fact]
    public void PokeApiClient_IsResolved()
        => _sp.GetService<PokeApiClient>().Should().NotBeNull().And.BeOfType<PokeApiClient>();
    
    [Fact]
    public void ITranslationClient_IsResolved()
        => _sp.GetService<ITranslationClient>().Should().NotBeNull().And.BeOfType<TranslationClient>();
    
    [Fact]
    public void IPokemonService_IsResolved()
        => _sp.GetService<IPokemonService>().Should().NotBeNull().And.BeOfType<PokemonService>();
    
    [Fact]
    public void ITranslationService_IsResolved()
        => _sp.GetService<ITranslationService>().Should().NotBeNull().And.BeOfType<TranslationService>();
}