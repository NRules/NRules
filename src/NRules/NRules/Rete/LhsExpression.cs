using System;
using System.Linq.Expressions;

namespace NRules.Rete
{
    internal interface ILhsExpression<out TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact);
    }

    internal interface ILhsFactExpression<out TResult> : ILhsExpression<TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Fact fact);
    }

    internal interface ILhsTupleExpression<out TResult> : ILhsExpression<TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple);
    }

    internal sealed class LhsExpression<TResult> : ILhsExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Tuple, Fact, TResult> _compiledExpression;

        public LhsExpression(LambdaExpression expression, Func<Tuple, Fact, TResult> compiledExpression)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression(tuple, fact);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, null, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                    context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, null, result, tuple, fact, nodeInfo);
            }
        }
    }

    internal sealed class LhsFactExpression<TResult> : ILhsFactExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Fact, TResult> _compiledExpression;

        public LhsFactExpression(LambdaExpression expression, Func<Fact, TResult> compiledExpression)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Fact fact)
        {
            return Invoke(context, nodeInfo, null, fact);
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression(fact);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, fact.Object, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                    context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, fact.Object, result, tuple, fact, nodeInfo);
            }
        }
    }

    internal sealed class LhsTupleExpression<TResult> : ILhsTupleExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Tuple, TResult> _compiledExpression;

        public LhsTupleExpression(LambdaExpression expression, Func<Tuple, TResult> compiledExpression)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple)
        {
            return Invoke(context, nodeInfo, tuple, null);
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression(tuple);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, null, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                    context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, null, result, tuple, fact, nodeInfo);
            }
        }
    }
}