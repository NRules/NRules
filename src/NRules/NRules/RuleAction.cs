using System;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        void Invoke(IContext context, Tuple tuple);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly TupleMask _tupleMask;
        private readonly Action<object[]> _compiledAction;

        public RuleAction(LambdaExpression expression, TupleMask tupleMask)
        {
            _tupleMask = tupleMask;
            _compiledAction = FastDelegate.Create<Action<object[]>>(expression);
        }

        public void Invoke(IContext context, Tuple tuple)
        {
            var args = new object[tuple.Count + 1];
            args[0] = context;
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                _tupleMask.SetAtIndex(ref args, index, 1, fact.Object);
                index--;
            }

            _compiledAction.Invoke(args);
        }
    }
}