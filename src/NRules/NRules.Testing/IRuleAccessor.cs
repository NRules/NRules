using System.Collections.Generic;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

internal interface IRuleAccessor
{
    IRuleMetadata GetRule();

    IRuleMetadata GetRule<T>() where T : Rule;

    IReadOnlyCollection<IMatch> GetFiredRuleMatches(string ruleName);
}
