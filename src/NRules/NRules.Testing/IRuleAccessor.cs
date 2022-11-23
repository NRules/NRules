using System;
using System.Collections.Generic;
using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Testing;

internal interface IRuleAccessor
{
    IReadOnlyCollection<Type> RegisteredRuleTypes { get; }

    IRuleMetadata GetRule(Type ruleType);

    IReadOnlyCollection<IMatch> GetFiredRuleMatches(string ruleName);
}
