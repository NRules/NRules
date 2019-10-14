using System.Collections.Generic;

namespace NRules.AgendaFilters
{
    internal class PredicateAgendaFilter : IAgendaFilter
    {
        private readonly List<IActivationExpression<bool>> _conditions;

        public PredicateAgendaFilter(IEnumerable<IActivationExpression<bool>> conditions)
        {
            _conditions = new List<IActivationExpression<bool>>(conditions);
        }

        public bool Accept(AgendaContext context, Activation activation)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.Invoke(context, activation)) return false;
            }
            return true;
        }
    }
}