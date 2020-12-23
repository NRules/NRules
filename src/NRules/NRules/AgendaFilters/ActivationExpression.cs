using System;
using System.Linq.Expressions;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules.AgendaFilters
{
    internal interface IActivationExpression<out TResult>
    {
        TResult Invoke(AgendaContext context, Activation activation);
    }

    internal class ActivationExpression<TResult> : IActivationExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Tuple, TResult> _compiledExpression;
        private readonly IArgumentMap _argumentMap;

        public ActivationExpression(LambdaExpression expression, Func<Tuple, TResult> compiledExpression, IArgumentMap argumentMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _argumentMap = argumentMap;
        }

        public TResult Invoke(AgendaContext context, Activation activation)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression.Invoke(activation.Tuple);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseAgendaExpressionFailed(context.Session, e, _expression, _argumentMap, activation, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                    context.EventAggregator.RaiseAgendaExpressionEvaluated(context.Session, exception, _expression, _argumentMap, result, activation);
            }
        }
    }
}