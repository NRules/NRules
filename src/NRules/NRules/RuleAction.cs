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
        void Invoke(IExecutionContext executionContext, IContext actionContext, Tuple tuple);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly TupleMask _tupleMask;
        private readonly Action<object[]> _compiledAction;

        public RuleAction(LambdaExpression expression, TupleMask tupleMask)
        {
            _expression = expression;
            _tupleMask = tupleMask;
            _compiledAction = FastDelegate.Create<Action<object[]>>(expression);
        }

        public void Invoke(IExecutionContext context, IContext actionContext, Tuple tuple)
        {
            var args = new object[tuple.Count + 1];
            args[0] = actionContext;
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                _tupleMask.SetAtIndex(ref args, index, 1, fact.Object);
                index--;
            }

            try
            {
                _compiledAction.Invoke(args);
            }
            catch (Exception e)
            {
                bool isHandled;
                context.EventAggregator.RaiseActionFailed(e, _expression, tuple, out isHandled);
                if (!isHandled)
                {
                    throw new RuleActionEvaluationException("Failed to evaluate rule action", _expression.ToString(), e);
                }
            }
        }
    }
}