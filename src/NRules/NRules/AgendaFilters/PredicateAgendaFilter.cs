namespace NRules.AgendaFilters;

internal class PredicateAgendaFilter : IAgendaFilter
{
    private readonly IReadOnlyCollection<IActivationExpression<bool>> _conditions;

    public PredicateAgendaFilter(IReadOnlyCollection<IActivationExpression<bool>> conditions)
    {
        _conditions = conditions;
    }

    public bool Accept(AgendaContext context, Activation activation)
    {
        foreach (var condition in _conditions)
        {
            if (!condition.Invoke(context, activation))
                return false;
        }
        return true;
    }
}