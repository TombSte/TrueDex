namespace TrueDex.Api.Strategies;

public static class StrategyKeys
{
    public const string Yoda = "Yoda";
    public const string Default =  "Default";

    public static string GetStrategyKey(string habitat)
    {
        return habitat switch
        {
            "cave" => Yoda,
            _ => Default
        };
    }
}