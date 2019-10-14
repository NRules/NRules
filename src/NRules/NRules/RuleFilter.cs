using System.Collections.Generic;
using NRules.AgendaFilters;

namespace NRules
{
    internal interface IRuleFilter
    {
        IEnumerable<IActivationExpression<bool>> Conditions { get; }
        IEnumerable<IActivationExpression<object>> KeySelectors { get; }
    }

    internal class RuleFilter : IRuleFilter
    {
        private readonly List<IActivationExpression<bool>> _conditions;
        private readonly List<IActivationExpression<object>> _keySelectors;

        public RuleFilter(IEnumerable<IActivationExpression<bool>> conditions, IEnumerable<IActivationExpression<object>> keySelectors)
        {
            _conditions = new List<IActivationExpression<bool>>(conditions);
            _keySelectors = new List<IActivationExpression<object>>(keySelectors);
        }

        public IEnumerable<IActivationExpression<bool>> Conditions => _conditions;
        public IEnumerable<IActivationExpression<object>> KeySelectors => _keySelectors;
    }
}
