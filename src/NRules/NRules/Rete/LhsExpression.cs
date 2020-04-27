using System;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface ILhsExpression<out TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, ITuple tuple, IFact fact);
    }

    internal interface ILhsFactExpression<out TResult> : ILhsExpression<TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, IFact fact);
    }

    internal interface ILhsTupleExpression<out TResult> : ILhsExpression<TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, ITuple tuple);
    }

    internal sealed class LhsExpression<TResult> : ILhsExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factMap;
        private readonly FastDelegate<Func<object[], TResult>> _compiledExpression;
        private readonly object[] _args;

        public LhsExpression(LambdaExpression expression, FastDelegate<Func<object[], TResult>> compiledExpression, IndexMap factMap)
        {
            _expression = expression;
            _factMap = factMap;
            _compiledExpression = compiledExpression;
            _args = new object[_compiledExpression.ArrayArgumentCount];
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, ITuple tuple, IFact fact)
        {
            int index = tuple.Count - 1;
            foreach (var tupleFact in tuple.Facts)
            {
                IndexMap.SetElementAt(_args, _factMap[index], tupleFact.Value);
                index--;
            }
            IndexMap.SetElementAt(_args, _factMap[tuple.Count], fact.Value);

            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression.Delegate(_args);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, _args, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, _args, result, tuple, fact, nodeInfo);
            }
        }
    }

    internal sealed class LhsFactExpression<TResult> : ILhsFactExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object, TResult>> _compiledExpression;

        public LhsFactExpression(LambdaExpression expression, FastDelegate<Func<object, TResult>> compiledExpression)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, IFact fact)
        {
            return Invoke(context, nodeInfo, null, fact);
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, ITuple tuple, IFact fact)
        {
            var factValue = fact.Value;
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression.Delegate(factValue);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, factValue, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, factValue, result, tuple, fact, nodeInfo);
            }
        }
    }

    internal sealed class LhsTupleExpression<TResult> : ILhsTupleExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object[], TResult>> _compiledExpression;
        private readonly IndexMap _factMap;

        public LhsTupleExpression(LambdaExpression expression, FastDelegate<Func<object[], TResult>> compiledExpression, IndexMap factMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _factMap = factMap;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, ITuple tuple)
        {
            return Invoke(context, nodeInfo, tuple, null);
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, ITuple tuple, IFact fact)
        {
            var args = new object[_compiledExpression.ArrayArgumentCount];
            int index = tuple.Count - 1;
            foreach (var tupleFact in tuple.Facts)
            {
                IndexMap.SetElementAt(args, _factMap[index], tupleFact.Value);
                index--;
            }

            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression.Delegate(args);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, args, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, args, result, tuple, fact, nodeInfo);
            }
        }
    }
}