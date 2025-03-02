using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules;

internal class ActionInvocation(IExecutionContext executionContext, IActionContext actionContext, IRuleAction action)
    : IActionInvocation
{
    public IReadOnlyList<object?> Arguments => action.GetArguments(actionContext);
    public Expression Expression => action.Expression;
    public ActionTrigger Trigger => action.Trigger;

    public void Invoke()
    {
        action.Invoke(executionContext, actionContext);
    }
}