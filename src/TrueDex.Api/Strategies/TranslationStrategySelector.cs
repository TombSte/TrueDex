namespace TrueDex.Api.Strategies;

public interface ITranslationStrategySelector
{
    ITranslationStrategy GetTranslationStrategy(string habitat);
    ITranslationStrategy GetDefaultTranslationStrategy();
}

public class TranslationStrategySelector(IServiceProvider sp) : ITranslationStrategySelector
{
    public ITranslationStrategy GetTranslationStrategy(string habitat) 
        => sp.GetRequiredKeyedService<ITranslationStrategy>(StrategyKeys.GetStrategyKey(habitat));

    public ITranslationStrategy GetDefaultTranslationStrategy()
        => sp.GetRequiredKeyedService<ITranslationStrategy>(StrategyKeys.Default);
}