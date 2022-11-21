using NRules.Fluent;

namespace NRules.Testing;

public interface IRuleFireAssertResult
{
    int Expected { get; }
    int Actual { get; }
    IRuleMetadata Rule { get; }
}
