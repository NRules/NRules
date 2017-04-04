using System.Collections.Generic;
using NRules.Extensibility;

namespace NRules
{
    internal interface IActionExecutor
    {
        void Execute(IExecutionContext executionContext, IActionContext actionContext);
    }

    internal class ActionExecutor : IActionExecutor
    {
        public void Execute(IExecutionContext executionContext, IActionContext actionContext)
        {
            ISession session = executionContext.Session;
            Activation activation = actionContext.Activation;

            var invocations = CreateInvocations(executionContext, actionContext);

            executionContext.EventAggregator.RaiseRuleFiring(session, activation);
            var interceptor = session.ActionInterceptor;
            if (interceptor != null)
            {
                interceptor.Intercept(actionContext, invocations);
            }
            else
            {
                foreach (var invocation in invocations)
                {
                    invocation.Invoke();
                }
            }
            executionContext.EventAggregator.RaiseRuleFired(session, activation);
        }

        private IEnumerable<IActionInvocation> CreateInvocations(IExecutionContext executionContext, IActionContext actionContext)
        {
            ICompiledRule compiledRule = actionContext.CompiledRule;
            var invocations = new List<IActionInvocation>();
            foreach (IRuleAction action in compiledRule.Actions)
            {
                var args = action.GetArguments(executionContext, actionContext);
                var invocation = new ActionInvocation(executionContext, actionContext, action, args);
                invocations.Add(invocation);
            }
            return invocations;
        }
    }
}
