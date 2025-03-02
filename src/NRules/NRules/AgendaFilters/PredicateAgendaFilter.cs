using System.Collections.Generic;

namespace NRules.AgendaFilters;

internal class PredicateAgendaFilter(IReadOnlyCollection<IActivationExpression<bool>> conditions)
    : IAgendaFilter
{
    public bool Accept(AgendaContext context, Activation activation)
    {
        foreach (var condition in conditions)
        {
            if (!condition.Invoke(context, activation)) return false;
        }
        return true;
    }
}