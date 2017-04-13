using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Extensibility
{
    /// <summary>
    /// Extension point for rule actions interception.
    /// </summary>
    /// <remarks>
    /// When actions are invoked via <c>IActionInterceptor</c>, exceptions thrown by actions
    /// are not wrapped into <see cref="RuleActionEvaluationException"/>. It is the responsibility
    /// of the interceptor to handle the exceptions.
    /// Exceptions thrown from the interceptor are not handled by the engine and just propagate up the stack.
    /// </remarks>
    public interface IActionInterceptor
    {
        /// <summary>
        /// Called by the rules engine in place of the action invocations when a rule fires.
        /// The interceptor can add behavior to action invocation and choose to either proceed with the invocations or not.
        /// </summary>
        /// <param name="context">Action context, containing information about the firing rule and matched facts.</param>
        /// <param name="actions">Action invocations for rule actions being intercepted.</param>
        void Intercept(IContext context, IEnumerable<IActionInvocation> actions);
    }
}
