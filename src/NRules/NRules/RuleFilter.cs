using System.Collections.Generic;
using NRules.AgendaFilters;

namespace NRules;

internal interface IRuleFilter
{
    IEnumerable<IActivationExpression<bool>> Conditions { get; }
    IEnumerable<IActivationExpression<object>> KeySelectors { get; }
}

internal class RuleFilter : IRuleFilter
{
    public RuleFilter(IReadOnlyCollection<IActivationExpression<bool>> conditions, IReadOnlyCollection<IActivationExpression<object>> keySelectors)
    {
        Conditions = conditions;
        KeySelectors = keySelectors;
    }

    public IEnumerable<IActivationExpression<bool>> Conditions { get; }
    public IEnumerable<IActivationExpression<object>> KeySelectors { get; }
}
