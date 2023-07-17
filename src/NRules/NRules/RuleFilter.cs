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
    private readonly IReadOnlyList<IActivationExpression<bool>> _conditions;
    private readonly IReadOnlyList<IActivationExpression<object>> _keySelectors;

    public RuleFilter(IReadOnlyList<IActivationExpression<bool>> conditions, IReadOnlyList<IActivationExpression<object>> keySelectors)
    {
        _conditions = conditions;
        _keySelectors = keySelectors;
    }

    public IReadOnlyList<IActivationExpression<bool>> Conditions => _conditions;
    public IReadOnlyList<IActivationExpression<object>> KeySelectors => _keySelectors;
}
