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
            return Yield(yield, yieldUpdate);
        }

        public IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yieldInsert, Expression<Func<IContext, TFact, TFact>> yieldUpdate)
        {
            _linkedCount++;
            var linkedFact = Expression.Parameter(typeof(TFact), "$temp");
            var linkedKey = Expression.Constant($"$linkedkey{_linkedCount}$");

            var insertContext = yieldInsert.Parameters[0];
            var insertAction = Expression.Lambda<Action<IContext>>(
                Expression.Block(
                    new[] {linkedFact},
                    Expression.Assign(linkedFact, Expression.Invoke(yieldInsert, insertContext)),
                    Expression.Call(insertContext,
                        typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.InsertLinked)),
                        linkedKey, linkedFact)),
                insertContext);

            var updateContext = yieldUpdate.Parameters[0];
            var updateAction = Expression.Lambda<Action<IContext>>(
                Expression.Block(
                    new[] {linkedFact},
                    Expression.Assign(linkedFact,
                        Expression.Convert(
                            Expression.Call(updateContext,
                                typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.GetLinked)),
                                linkedKey),
                            typeof(TFact))),
                    Expression.Assign(linkedFact, Expression.Invoke(yieldUpdate, updateContext, linkedFact)),
                    Expression.Call(updateContext,
                        typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.UpdateLinked)),
                        linkedKey, linkedFact)),
                updateContext);

            var rhs = Action(insertAction, ActionTrigger.Activated)
                .Action(updateAction, ActionTrigger.Reactivated);
            return rhs;
        }
    }
}