using NRules.Extensibility;

namespace NRules
{
    internal class ActionInvocation : IActionInvocation
    {
        private readonly IExecutionContext _executionContext;
        private readonly IActionContext _actionContext;

        public ActionInvocation(IExecutionContext executionContext, IActionContext actionContext, IRuleAction action, object[] arguments)
        {
            _executionContext = executionContext;
            _actionContext = actionContext;
            RuleAction = action;
            Arguments = arguments;
        }

        public object[] Arguments { get; }
        public IRuleAction RuleAction { get; }

        public void Invoke()
        {
            RuleAction.Invoke(_executionContext, _actionContext, Arguments);
        }
    }
}