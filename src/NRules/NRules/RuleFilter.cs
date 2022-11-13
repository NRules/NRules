using NRules.AgendaFilters;

namespace NRules;

internal interface IRuleFilter
{
    IReadOnlyCollection<IActivationExpression<bool>> Conditions { get; }
    IReadOnlyCollection<IActivationExpression<object>> KeySelectors { get; }
}

internal class RuleFilter : IRuleFilter
{
    private readonly IReadOnlyCollection<IActivationExpression<bool>> _conditions;
    private readonly IReadOnlyCollection<IActivationExpression<object>> _keySelectors;

    public RuleFilter(IReadOnlyCollection<IActivationExpression<bool>> conditions, IReadOnlyCollection<IActivationExpression<object>> keySelectors)
    {
        _conditions = conditions;
        _keySelectors = keySelectors;
    }

    public IReadOnlyCollection<IActivationExpression<bool>> Conditions => _conditions;
    public IReadOnlyCollection<IActivationExpression<object>> KeySelectors => _keySelectors;
}
