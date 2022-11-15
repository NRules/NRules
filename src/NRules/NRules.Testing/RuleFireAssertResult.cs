using NRules.Fluent;

namespace NRules.Testing
{
    internal class RuleFireAssertResult : IRuleFireAssertResult
    {
        public RuleFireAssertResult(IRuleMetadata rule, int expected, int actual)
        {
            Rule = rule;
            Expected = expected;
            Actual = actual;
        }

        public IRuleMetadata Rule { get; }
        public int Expected { get; }
        public int Actual { get; }
    }
}
