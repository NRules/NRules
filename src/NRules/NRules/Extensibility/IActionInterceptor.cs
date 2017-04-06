using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Extensibility
{
    /// <summary>
    /// Extension point for rule actions interception.
    /// </summary>
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
