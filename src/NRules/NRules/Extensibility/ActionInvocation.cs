using System;
using NRules.RuleModel;

namespace NRules.Extensibility
{
    /// <summary>
    /// Represents invocation of the proxied rule action.
    /// </summary>
    public interface IActionInvocation
    {
        /// <summary>
        /// Rules engine context.
        /// </summary>
        IContext Context { get; }

        /// <summary>
        /// Action arguments.
        /// </summary>
        /// <remarks>Action arguments also include dependencies that are passed to the action method.</remarks>
        /// <remarks>Action arguments don't include <c>IContext</c>; it is supplied as <see cref="Context"/> property.</remarks>
        object[] Arguments { get; }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        void Invoke();
    }

    internal class ActionInvocation : IActionInvocation
    {
        private readonly IActionContext _context;
        private readonly object[] _arguments;
        private readonly Action<IContext, object[]> _action;

        public ActionInvocation(IActionContext context, object[] arguments, Action<IContext, object[]> action)
        {
            _context = context;
            _arguments = arguments;
            _action = action;
        }

        public IContext Context { get { return _context; } }
        public object[] Arguments { get { return _arguments; } }

        public void Invoke()
        {
            _action.Invoke(_context, _arguments);
        }
    }
}