using System;
using NRules.Dsl;

namespace NRules.Rule
{
    public interface IRuleAction
    {
        void Invoke(IActionContext context);
    }

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