using System;
using System.Linq.Expressions;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules.AgendaFilters;

internal interface IActivationExpression<out TResult>
{
    TResult Invoke(AgendaContext context, Activation activation);
}

internal class ActivationExpression<TResult>(
    LambdaExpression expression,
    Func<Tuple, TResult> compiledExpression,
    IArgumentMap argumentMap)
    : IActivationExpression<TResult>
{
    public TResult Invoke(AgendaContext context, Activation activation)
    {
        Exception? exception = null;
        TResult? result = default;
        try
        {
            result = compiledExpression.Invoke(activation.Tuple);
            return result;
        }
        catch (Exception e)
        {
            exception = e;
            bool isHandled = false;
            context.EventAggregator.RaiseAgendaExpressionFailed(context.Session, e, expression, argumentMap, activation, ref isHandled);
            throw new ExpressionEvaluationException(e, expression, isHandled);
        }
        finally
        {
            if (context.EventAggregator.TraceEnabled)
                context.EventAggregator.RaiseAgendaExpressionEvaluated(context.Session, exception, expression, argumentMap, result, activation);
        }
    }
}