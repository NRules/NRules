using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        void Invoke(IContext context, Tuple tuple);
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

        public void Invoke(IContext context, Tuple tuple)
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