using System.Collections.Generic;

namespace NRules.AgendaFilters
{
    internal class PredicateAgendaFilter : IAgendaFilter
    {
        private readonly List<IActivationCondition> _conditions;

        public PredicateAgendaFilter(IEnumerable<IActivationCondition> conditions)
        {
            _conditions = new List<IActivationCondition>(conditions);
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