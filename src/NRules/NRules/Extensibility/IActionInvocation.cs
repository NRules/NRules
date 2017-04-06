namespace NRules.Extensibility
{
    /// <summary>
    /// Represents invocation of the proxied rule action.
    /// </summary>
    public interface IActionInvocation
    {
        /// <summary>
        /// Action arguments.
        /// </summary>
        /// <remarks>Action arguments also include dependencies that are passed to the action method.</remarks>
        /// <remarks>Action arguments don't include <c>IContext</c>.</remarks>
        object[] Arguments { get; }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        void Invoke();
    }
}