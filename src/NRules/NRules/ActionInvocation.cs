using NRules.Extensibility;

namespace NRules
{
    internal class ActionInvocation : IActionInvocation
    {
        private readonly IExecutionContext _executionContext;
        private readonly IActionContext _actionContext;
        private readonly IRuleAction _action;
        private readonly object[] _arguments;

        public ActionInvocation(IExecutionContext executionContext, IActionContext actionContext, IRuleAction action, object[] arguments)
        {
            _executionContext = executionContext;
            _actionContext = actionContext;
            _action = action;
            _arguments = arguments;
        }

        public object[] Arguments
        {
            get { return _arguments; }
        }

        public IRuleAction RuleAction
        {
            get { return _action; }
        }

        public void Invoke()
        {
            _action.Invoke(_executionContext, _actionContext, _arguments);
        }
    }
}