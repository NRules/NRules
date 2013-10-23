using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Core.Rete;
using Tuple = NRules.Core.Rete.Tuple;

namespace NRules.Core
{
    internal interface IRuleAction
    {
        void Invoke(IActionContext context, Tuple tuple);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly int[] _tupleMask;
        private readonly Delegate _compiledAction;

        public RuleAction(LambdaExpression expression, int[] tupleMask)
        {
            _tupleMask = tupleMask;
            _compiledAction = expression.Compile();
        }

        public void Invoke(IActionContext context, Tuple tuple)
        {
            //todo: optimize
            IEnumerable<Fact> facts =
                _tupleMask.Select(
                    idx => tuple.ElementAtOrDefault(idx));
            object[] args = Enumerable.Repeat(context, 1).Union(facts.Select(f => f.Object)).ToArray();
            _compiledAction.DynamicInvoke(args);
        }
    }
}