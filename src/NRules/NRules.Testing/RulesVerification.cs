using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

internal class RulesVerification : IRulesVerification
{
    private readonly IRuleAsseter _asseter;
    private readonly IRuleAccessor _accessor;

    public RulesVerification(IRuleAsseter asseter, IRuleAccessor accessor)
    {
        _asseter = asseter;
        _accessor = accessor;
    }

    public ISelectedRuleVerification Rule()
    {
        var ruleMetadata = _accessor.GetRule();
        return Rule(ruleMetadata);
    }

    public ISelectedRuleVerification Rule<T>() where T : Rule
    {
        var ruleMetadata = _accessor.GetRule<T>();
        return Rule(ruleMetadata);
    }

    private ISelectedRuleVerification Rule(IRuleMetadata ruleMetadata)
    {
        var matches = _accessor.GetFiredRuleMatches(ruleMetadata.Name);
        return new RuleVerification(_asseter, ruleMetadata, matches);
    }
}
