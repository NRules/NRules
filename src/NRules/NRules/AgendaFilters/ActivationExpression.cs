using System;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.Utilities;

namespace NRules.AgendaFilters
{
    internal interface IActivationExpression<out TResult>
    {
        TResult Invoke(AgendaContext context, Activation activation);
    }

    internal class ActivationExpression<TResult> : IActivationExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object[], TResult>> _compiledExpression;
        private readonly IndexMap _tupleFactMap;

        public ActivationExpression(LambdaExpression expression, FastDelegate<Func<object[], TResult>> compiledExpression, IndexMap tupleFactMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _tupleFactMap = tupleFactMap;
        }

        public TResult Invoke(AgendaContext context, Activation activation)
        {
            var tuple = activation.Tuple;
            var activationFactMap = activation.FactMap;

            var args = new object[_compiledExpression.ArrayArgumentCount];

            int index = tuple.Count - 1;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = _tupleFactMap[activationFactMap[index]];
                IndexMap.SetElementAt(args, mappedIndex, fact.Object);
                index--;
            }

            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression.Delegate.Invoke(args);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseAgendaExpressionFailed(context.Session, e, _expression, args, activation, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseAgendaExpressionEvaluated(context.Session, exception, _expression, args, result, activation);
            }
        }
    }
}