using System;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Core
{
    internal interface IRuleAction
    {
        void Invoke(IActionContext context);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly Type[] _argumentTypes;
        private readonly Delegate _compiledAction;

        public RuleAction(LambdaExpression expression)
        {
            _expression = expression;
            _argumentTypes = _expression.Parameters.Skip(1).Select(p => p.Type).ToArray();
            _compiledAction = expression.Compile();
        }

        public void Invoke(IActionContext context)
        {
            var args = Enumerable.Repeat(context, 1).Union(_argumentTypes.Select(context.Get)).ToArray();
            _compiledAction.DynamicInvoke(args);
        }
    }
}