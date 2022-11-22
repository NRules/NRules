using System.Collections.Generic;
using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Testing;

internal class RuleVerification : ISelectedRuleVerification
{
    private readonly IRuleAsseter _asseter;
    private readonly IRuleMetadata _ruleMetadata;
    private readonly IReadOnlyCollection<IMatch> _matches;

    public RuleVerification(IRuleAsseter asseter, IRuleMetadata ruleMetadata, IReadOnlyCollection<IMatch> matches)
    {
        _asseter = asseter;
        _ruleMetadata = ruleMetadata;
        _matches = matches;
    }

    public void FiredTimes(int expected)
    {
        var result = GetAssertResult(_ruleMetadata, expected);
        _asseter.Assert(result);
    }

    private RuleFireAssertResult GetAssertResult(IRuleMetadata ruleMetadata, int expected)
        => new(ruleMetadata, expected, _matches.Count);
}
