using System;
using NRules.Dsl;

namespace NRules.Core
{
    internal interface IRuleAction
    {
        void Invoke(IActionContext context);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly Action<IActionContext> _compiledAction;

        public RuleAction(Action<IActionContext> compiledAction)
        {
            _compiledAction = compiledAction;
        }

        public void Invoke(IActionContext context)
        {
            _compiledAction.Invoke(context);
        }
    }
}