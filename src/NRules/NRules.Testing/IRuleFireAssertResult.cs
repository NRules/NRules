using NRules.Fluent;

public interface IRuleFireAssertResult
{
    int Expected { get; }
    int Actual { get; }
    IRuleMetadata Rule { get; }
}
