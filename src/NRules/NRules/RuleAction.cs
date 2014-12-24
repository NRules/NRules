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
        void Invoke(IExecutionContext executionContext, IContext actionContext, Tuple tuple, FactIndexMap tupleFactMap);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly FactIndexMap _actionFactMap;
        private readonly FastDelegate<Action<object[]>> _compiledAction;

        public RuleAction(LambdaExpression expression, FactIndexMap actionFactMap)
        {
            _expression = expression;
            _actionFactMap = actionFactMap;
            _compiledAction = FastDelegate.Create<Action<object[]>>(expression);
        }

        public void Invoke(IExecutionContext context, IContext actionContext, Tuple tuple, FactIndexMap tupleFactMap)
        {
            var args = new object[_compiledAction.ParameterCount];
            args[0] = actionContext;
            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = _actionFactMap.Map(tupleFactMap.Map(index));
                FactIndexMap.SetElementAt(ref args, mappedIndex, 1, fact.Object);
                index--;
            }

            try
            {
                _compiledAction.Delegate.Invoke(args);
            }
            catch (Exception e)
            {
                bool isHandled;
                context.EventAggregator.RaiseActionFailed(context.Session, e, _expression, tuple, out isHandled);
                if (!isHandled)
                {
                    throw new RuleActionEvaluationException("Failed to evaluate rule action", _expression.ToString(), e);
                }
            }
        }
    }
}