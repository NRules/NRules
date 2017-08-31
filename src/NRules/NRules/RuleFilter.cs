using System.Collections.Generic;

namespace NRules
{
    internal interface IRuleFilter
    {
        IEnumerable<IActivationCondition> Conditions { get; }
        IEnumerable<IActivationExpression> KeySelectors { get; }
    }

    internal class RuleFilter : IRuleFilter
    {
        private readonly List<IActivationCondition> _conditions;
        private readonly List<IActivationExpression> _keySelectors;

        public RuleFilter(IEnumerable<IActivationCondition> conditions, IEnumerable<IActivationExpression> keySelectors)
        {
            _conditions = new List<IActivationCondition>(conditions);
            _keySelectors = new List<IActivationExpression>(keySelectors);
        }

        public IEnumerable<IActivationCondition> Conditions => _conditions;
        public IEnumerable<IActivationExpression> KeySelectors => _keySelectors;
    }
}
