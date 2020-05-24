using System.Linq.Expressions;
using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules
{
    internal class ActionInvocation : IActionInvocation
    {
        private readonly IExecutionContext _executionContext;
        private readonly IActionContext _actionContext;
        private readonly IRuleAction _action;

        public ActionInvocation(IExecutionContext executionContext, IActionContext actionContext, IRuleAction action)
        {
            _executionContext = executionContext;
            _actionContext = actionContext;
            _action = action;
        }

        public object[] Arguments => _action.GetArguments(_actionContext);
        public Expression Expression => _action.Expression;
        public ActionTrigger Trigger => _action.Trigger;

        public void Invoke()
        {
            _action.Invoke(_executionContext, _actionContext);
        }
    }
}