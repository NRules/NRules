using System;
using System.Linq.Expressions;
using System.Reflection;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class RightHandSideExpression : IRightHandSideExpression
    {
        private int _linkedCount = 0;
        private readonly ActionGroupBuilder _builder;
        private readonly SymbolStack _symbolStack;

        public RightHandSideExpression(ActionGroupBuilder builder, SymbolStack symbolStack)
        {
            _builder = builder;
            _symbolStack = symbolStack;
        }

        public IRightHandSideExpression Action(Expression<Action<IContext>> action, ActionTrigger actionTrigger)
        {
            _builder.DslAction(_symbolStack.Scope.Declarations, action, actionTrigger);
            return this;
        }

        public IRightHandSideExpression Do(Expression<Action<IContext>> action)
        {
            return Action(action, ActionTrigger.Activated | ActionTrigger.Reactivated);
        }

        public IRightHandSideExpression Undo(Expression<Action<IContext>> action)
        {
            return Action(action, ActionTrigger.Deactivated);
        }

        public IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yield)
        {
            var context = yield.Parameters[0];
            var linkedFact = Expression.Parameter(typeof(TFact));
            var yieldUpdate = Expression.Lambda<Func<IContext, TFact, TFact>>(yield.Body, context, linkedFact);
            var action = CreateYieldAction(yield, yieldUpdate);
            return Do(action);
        }

        public IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yieldInsert, Expression<Func<IContext, TFact, TFact>> yieldUpdate)
        {
            var action = CreateYieldAction(yieldInsert, yieldUpdate);
            return Do(action);
        }

        public Expression<Action<IContext>> CreateYieldAction<TFact>(Expression<Func<IContext, TFact>> yieldInsert, Expression<Func<IContext, TFact, TFact>> yieldUpdate)
        {
            _linkedCount++;
            var context = yieldInsert.Parameters[0];
            var linkedFact = Expression.Parameter(typeof(TFact));
            var linkedKey = Expression.Constant($"$linkedkey{_linkedCount}$");

            var action = Expression.Lambda<Action<IContext>>(
                Expression.Block(
                    new[] {linkedFact},
                    Expression.Assign(linkedFact,
                        Expression.Convert(
                            Expression.Call(context,
                                typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.GetLinked)),
                                linkedKey),
                            typeof(TFact))),
                    Expression.IfThenElse(
                        Expression.Equal(linkedFact, Expression.Constant(null)),
                        Expression.Call(context,
                            typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.InsertLinked)), linkedKey,
                            yieldInsert.Body),
                        Expression.Block(
                            Expression.Assign(linkedFact, Expression.Invoke(yieldUpdate, context, linkedFact)),
                            Expression.Call(context,
                                typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.UpdateLinked)),
                                linkedKey, linkedFact)))
                ),
                context);
            return action;
        }
    }
}