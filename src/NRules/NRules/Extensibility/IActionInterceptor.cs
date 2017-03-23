namespace NRules.Extensibility
{
    /// <summary>
    /// Extension point for rule action interception.
    /// </summary>
    public interface IActionInterceptor
    {
        /// <summary>
        /// Called by the rules engine when rule fires, instead of the action invocation.
        /// The interceptor can add behavior to action invocation and choose to either proceed with the invocation or not.
        /// </summary>
        /// <remarks>If a rule has multiple actions, the interceptor is called once for every action.</remarks>
        /// <param name="action">Action invocation object for the action being intercepted.</param>
        void Intercept(IActionInvocation action);
    }
}
