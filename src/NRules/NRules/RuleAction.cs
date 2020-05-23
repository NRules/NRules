using System;
using System.Linq.Expressions;
using NRules.Extensibility;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        Expression Expression { get; }
        ActionTrigger Trigger { get; }
        void Invoke(IExecutionContext executionContext, IActionContext actionContext);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly Action<IContext, Tuple> _compiledExpression;

        public RuleAction(LambdaExpression expression, Action<IContext, Tuple> compiledExpression,
            ActionTrigger actionTrigger)
        {
            _expression = expression;
            Trigger = actionTrigger;
            _compiledExpression = compiledExpression;
        }

        public Expression Expression => _expression;
        public ActionTrigger Trigger { get; }

        public void Invoke(IExecutionContext executionContext, IActionContext actionContext)
        {
            var activation = actionContext.Activation;
            var tuple = activation.Tuple;

            Exception exception = null;
            try
            {
                _compiledExpression.Invoke(actionContext, tuple);
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                executionContext.EventAggregator.RaiseRhsExpressionFailed(executionContext.Session, e, _expression, null, actionContext.Activation, ref isHandled);
                if (!isHandled)
                {
                    throw;
                }
            }
            finally
            {
                if (executionContext.EventAggregator.TraceEnabled)
                    executionContext.EventAggregator.RaiseRhsExpressionEvaluated(executionContext.Session, exception, _expression, null, actionContext.Activation);
            }
        }
    }

    internal class RuleActionWithDependencies : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly Action<IContext, Tuple, IDependencyResolver, IResolutionContext> _compiledExpression;

        public RuleActionWithDependencies(LambdaExpression expression, Action<IContext, Tuple, IDependencyResolver, IResolutionContext> compiledExpression,
            ActionTrigger actionTrigger)
        {
            _expression = expression;
            Trigger = actionTrigger;
            _compiledExpression = compiledExpression;
        }

        public Expression Expression => _expression;
        public ActionTrigger Trigger { get; }

        public void Invoke(IExecutionContext executionContext, IActionContext actionContext)
        {
            var compiledRule = actionContext.CompiledRule;
            var activation = actionContext.Activation;
            var tuple = activation.Tuple;

            var dependencyResolver = executionContext.Session.DependencyResolver;
            var resolutionContext = new ResolutionContext(executionContext.Session, compiledRule.Definition);

            Exception exception = null;
            try
            {
                _compiledExpression.Invoke(actionContext, tuple, dependencyResolver, resolutionContext);
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                executionContext.EventAggregator.RaiseRhsExpressionFailed(executionContext.Session, e, _expression, null, actionContext.Activation, ref isHandled);
                if (!isHandled)
                {
                    throw;
                }
            }
            finally
            {
                if (executionContext.EventAggregator.TraceEnabled)
                    executionContext.EventAggregator.RaiseRhsExpressionEvaluated(executionContext.Session, exception, _expression, null, actionContext.Activation);
            }
        }
    }
}
