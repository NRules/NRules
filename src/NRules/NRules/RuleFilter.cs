using System.Collections.Generic;
using NRules.AgendaFilters;

namespace NRules;

internal interface IRuleFilter
{
    IReadOnlyList<IActivationExpression<bool>> Conditions { get; }
    IReadOnlyList<IActivationExpression<object>> KeySelectors { get; }
}

internal class RuleFilter(
    IReadOnlyList<IActivationExpression<bool>> conditions,
    IReadOnlyList<IActivationExpression<object>> keySelectors)
    : IRuleFilter
{
    public IReadOnlyList<IActivationExpression<bool>> Conditions { get; } = conditions;
    public IReadOnlyList<IActivationExpression<object>> KeySelectors { get; } = keySelectors;
}
