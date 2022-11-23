using System.Collections.Generic;
using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Testing;

internal class RuleVerification : IRuleVerification
{
    private readonly IRuleAsserter _asserter;
    private readonly IRuleMetadata _ruleMetadata;
    private readonly IReadOnlyCollection<IMatch> _matches;

    public RuleVerification(IRuleAsserter asserter, IRuleMetadata ruleMetadata, IReadOnlyCollection<IMatch> matches)
    {
        _asserter = asserter;
        _ruleMetadata = ruleMetadata;
        _matches = matches;
    }

    public void FiredTimes(int expected)
    {
        var result = GetAssertResult(_ruleMetadata, expected);
        _asserter.Assert(result);
    }

    private RuleFireAssertResult GetAssertResult(IRuleMetadata ruleMetadata, int expected)
        => new(ruleMetadata, expected, _matches.Count);
}
