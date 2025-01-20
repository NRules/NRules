using System;
using System.Linq.Expressions;
using NRules.Diagnostics;
using NRules.Utilities;

namespace NRules.Rete;

internal interface ILhsExpression<out TResult>
{
    TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Tuple tuple, Fact fact);
}

internal interface ILhsFactExpression<out TResult> : ILhsExpression<TResult>
{
    TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Fact fact);
}

internal interface ILhsTupleExpression<out TResult> : ILhsExpression<TResult>
{
    TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Tuple tuple);
}

internal sealed class LhsExpression<TResult>(
    LambdaExpression expression,
    Func<Tuple, Fact, TResult> compiledExpression,
    IArgumentMap argumentMap)
    : ILhsExpression<TResult>
{
    public TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Tuple tuple, Fact fact)
    {
        Exception? exception = null;
        TResult? result = default;
        try
        {
            result = compiledExpression(tuple, fact);
            return result;
        }
        catch (Exception e)
        {
            exception = e;
            bool isHandled = false;
            context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, expression, argumentMap, tuple, fact, nodeInfo, ref isHandled);
            throw new ExpressionEvaluationException(e, expression, isHandled);
        }
        finally
        {
            if (context.EventAggregator.TraceEnabled)
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, expression, argumentMap, result, tuple, fact, nodeInfo);
        }
    }
}

internal sealed class LhsFactExpression<TResult>(
    LambdaExpression expression,
    Func<Fact, TResult> compiledExpression,
    IArgumentMap argumentMap)
    : ILhsFactExpression<TResult>
{
    public TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Fact fact)
    {
        return Invoke(context, nodeInfo, null, fact);
    }

    public TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Tuple? tuple, Fact fact)
    {
        Exception? exception = null;
        TResult? result = default;
        try
        {
            result = compiledExpression(fact);
            return result;
        }
        catch (Exception e)
        {
            exception = e;
            bool isHandled = false;
            context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, expression, argumentMap, tuple, fact, nodeInfo, ref isHandled);
            throw new ExpressionEvaluationException(e, expression, isHandled);
        }
        finally
        {
            if (context.EventAggregator.TraceEnabled)
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, expression, argumentMap, result, tuple, fact, nodeInfo);
        }
    }
}

internal sealed class LhsTupleExpression<TResult>(
    LambdaExpression expression,
    Func<Tuple, TResult> compiledExpression,
    IArgumentMap argumentMap)
    : ILhsTupleExpression<TResult>
{
    public TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Tuple tuple)
    {
        return Invoke(context, nodeInfo, tuple, null);
    }

    public TResult Invoke(IExecutionContext context, NodeInfo nodeInfo, Tuple tuple, Fact? fact)
    {
        Exception? exception = null;
        TResult? result = default;
        try
        {
            result = compiledExpression(tuple);
            return result;
        }
        catch (Exception e)
        {
            exception = e;
            bool isHandled = false;
            context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, expression, argumentMap, tuple, fact, nodeInfo, ref isHandled);
            throw new ExpressionEvaluationException(e, expression, isHandled);
        }
        finally
        {
            if (context.EventAggregator.TraceEnabled)
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, expression, argumentMap, result, tuple, fact, nodeInfo);
        }
    }
}