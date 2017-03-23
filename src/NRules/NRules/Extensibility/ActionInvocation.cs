using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

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
        /// Facts matched by the rule.
        /// </summary>
        /// <remarks>Not all facts matched by the rule are necessarily passed to every action.</remarks>
        IEnumerable<FactInfo> Facts { get; }

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
        private readonly Tuple _tuple;
        private readonly object[] _arguments;
        private readonly Action<IContext, object[]> _action;

        public ActionInvocation(IActionContext context, Tuple tuple, object[] arguments, Action<IContext, object[]> action)
        {
            _context = context;
            _tuple = tuple;
            _arguments = arguments;
            _action = action;
        }

        public IContext Context { get { return _context; } }

        public object[] Arguments { get { return _arguments; } }

        public IEnumerable<FactInfo> Facts
        {
            get { return _tuple.OrderedFacts.Select(f => new FactInfo(f)).ToArray(); }
        }

        public void Invoke()
        {
            _action.Invoke(_context, _arguments);
        }
    }
}