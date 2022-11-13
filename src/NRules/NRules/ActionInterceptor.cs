using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules;

internal class ActionInterceptor : IActionInterceptor
{
    public static IActionInterceptor Default { get; } = new ActionInterceptor();

    public void Intercept(IContext context, IEnumerable<IActionInvocation> actions)
    {
        foreach (var invocation in actions)
        {
            try
            {
                invocation.Invoke();
            }
            catch (Exception e)
            {
                throw new RuleRhsExpressionEvaluationException("Failed to evaluate rule action", context.Rule.Name, invocation.ToString(), e);
            }
        }
    }
}
