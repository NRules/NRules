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
        private readonly RuleBuilder _builder;

        public RightHandSideExpression(RuleBuilder builder)
        {
            _builder = builder;
        }

        public IRightHandSideExpression Do(Expression<Action<IContext>> action)
        {
            var rightHandSide = _builder.RightHandSide();
            rightHandSide.DslAction(rightHandSide.Declarations, action);
            return this;
        }

        public IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yield)
        {
            _linkedCount++;
            var context = yield.Parameters[0];
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
                            yield.Body),
                        Expression.Call(context,
                            typeof(IContext).GetTypeInfo().GetDeclaredMethod(nameof(IContext.UpdateLinked)), linkedKey,
                            yield.Body))
                ),
                context);
            return Do(action);
        }

        public IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yieldInsert, Expression<Func<IContext, TFact, TFact>> yieldUpdate)
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
            return Do(action);
        }
    }
}