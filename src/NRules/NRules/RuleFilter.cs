using System.Collections.Generic;
using NRules.AgendaFilters;

namespace NRules;

internal interface IRuleFilter
{
    IReadOnlyList<IActivationExpression<bool>> Conditions { get; }
    IReadOnlyList<IActivationExpression<object>> KeySelectors { get; }
}

internal class RuleFilter : IRuleFilter
{
    public RuleFilter(IReadOnlyList<IActivationExpression<bool>> conditions, IReadOnlyList<IActivationExpression<object>> keySelectors)
    {
        Conditions = conditions;
        KeySelectors = keySelectors;
    }

    public IReadOnlyList<IActivationExpression<bool>> Conditions { get; }
    public IReadOnlyList<IActivationExpression<object>> KeySelectors { get; }
}
