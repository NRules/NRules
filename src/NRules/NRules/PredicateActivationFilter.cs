using System.Collections.Generic;

namespace NRules
{
    internal class PredicateActivationFilter : IActivationFilter
    {
        private readonly List<IActivationCondition> _conditions;

        public PredicateActivationFilter(IEnumerable<IActivationCondition> conditions)
        {
            _conditions = new List<IActivationCondition>(conditions);
        }

        public bool Accept(Activation activation)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.Invoke(activation)) return false;
            }
            return true;
        }

        public void Remove(Activation activation)
        {
        }
    }
}