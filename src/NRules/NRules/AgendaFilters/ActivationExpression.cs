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
        private readonly IndexMap _expressionFactMap;

        public ActivationExpression(LambdaExpression expression, FastDelegate<Func<object[], TResult>> compiledExpression, IndexMap expressionFactMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _expressionFactMap = expressionFactMap;
        }

        public TResult Invoke(AgendaContext context, Activation activation)
        {
            var tuple = activation.Tuple;
            var tupleFactMap = activation.FactMap;

            var args = new object[_compiledExpression.ArrayArgumentCount];

            int index = tuple.Count - 1;
            var enumerator = tuple.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var mappedIndex = _expressionFactMap[tupleFactMap[index]];
                IndexMap.SetElementAt(args, mappedIndex, enumerator.Current.Object);
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
                if (context.EventAggregator.TraceEnabled)
                    context.EventAggregator.RaiseAgendaExpressionEvaluated(context.Session, exception, _expression, args, result, activation);
            }
        }
    }
}