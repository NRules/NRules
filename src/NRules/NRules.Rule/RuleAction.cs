using System;
using NRules.Fluent;

namespace NRules.Rule
{
    internal class RuleAction : IRuleAction
    {
        private readonly Action<IActionContext> _action;

        public RuleAction(Action<IActionContext> action)
        {
            _action = action;
        }

        public void Invoke(IActionContext context)
        {
            _action.Invoke(context);
        }
    }
}