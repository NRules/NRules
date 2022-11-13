using NRules.Utilities;

namespace NRules;

internal interface IActionExecutor
{
    void Execute(IExecutionContext executionContext, IActionContext actionContext);
}

internal class ActionExecutor : IActionExecutor
{
    public void Execute(IExecutionContext executionContext, IActionContext actionContext)
    {
        var invocations = CreateInvocations(executionContext, actionContext);

        var session = executionContext.Session;
        var activation = actionContext.Activation;

        executionContext.EventAggregator.RaiseRuleFiring(session, activation);

        session.ActionInterceptor.Intercept(actionContext, invocations);

        executionContext.EventAggregator.RaiseRuleFired(session, activation);
    }

    private static IEnumerable<ActionInvocation> CreateInvocations(IExecutionContext executionContext, IActionContext actionContext)
    {
        var trigger = actionContext.Activation.Trigger;
        return actionContext.CompiledRule.Actions
            .Where(a => trigger.Matches(a.Trigger))
            .Select(a => new ActionInvocation(executionContext, actionContext, a));
    }
}
