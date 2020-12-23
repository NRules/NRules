using System;
using System.Linq.Expressions;
using NRules.Extensibility;
using NRules.RuleModel;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    internal interface IRuleAction
    {
        Expression Expression { get; }
        ActionTrigger Trigger { get; }
        object[] GetArguments(IActionContext actionContext);
        void Invoke(IExecutionContext executionContext, IActionContext actionContext);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly Action<IContext, Tuple> _compiledExpression;
        private readonly IArgumentMap _argumentMap;

        public RuleAction(LambdaExpression expression, Action<IContext, Tuple> compiledExpression,
            IArgumentMap argumentMap, ActionTrigger actionTrigger)
        {
            _expression = expression;
            Trigger = actionTrigger;
            _compiledExpression = compiledExpression;
            _argumentMap = argumentMap;
        }

        public Expression Expression => _expression;
        public ActionTrigger Trigger { get; }

        public object[] GetArguments(IActionContext actionContext)
        {
            var arguments = new ActivationExpressionArguments(_argumentMap, actionContext.Activation);
            return arguments.GetValues();
        }

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
                executionContext.EventAggregator.RaiseRhsExpressionFailed(executionContext.Session, e, _expression, _argumentMap, actionContext.Activation, ref isHandled);
                if (!isHandled)
                {
                    throw;
                }
            }
            finally
            {
                if (executionContext.EventAggregator.TraceEnabled)
                    executionContext.EventAggregator.RaiseRhsExpressionEvaluated(executionContext.Session, exception, _expression, _argumentMap, actionContext.Activation);
            }
        }
    }

    internal class RuleActionWithDependencies : IRuleAction
    {
        private readonly LambdaExpression _expression;
        private readonly Action<IContext, Tuple, IDependencyResolver, IResolutionContext> _compiledExpression;
        private readonly IArgumentMap _argumentMap;

        public RuleActionWithDependencies(LambdaExpression expression, Action<IContext, Tuple, IDependencyResolver, IResolutionContext> compiledExpression,
            IArgumentMap argumentMap, ActionTrigger actionTrigger)
        {
            _expression = expression;
            Trigger = actionTrigger;
            _compiledExpression = compiledExpression;
            _argumentMap = argumentMap;
        }

        public Expression Expression => _expression;
        public ActionTrigger Trigger { get; }
        
        public object[] GetArguments(IActionContext actionContext)
        {
            var arguments = new ActivationExpressionArguments(_argumentMap, actionContext.Activation);
            return arguments.GetValues();
        }

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
                executionContext.EventAggregator.RaiseRhsExpressionFailed(executionContext.Session, e, _expression, _argumentMap, actionContext.Activation, ref isHandled);
                if (!isHandled)
                {
                    throw;
                }
            }
            finally
            {
                if (executionContext.EventAggregator.TraceEnabled)
                    executionContext.EventAggregator.RaiseRhsExpressionEvaluated(executionContext.Session, exception, _expression, _argumentMap, actionContext.Activation);
            }
        }
    }
}
