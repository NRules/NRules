using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's right-hand side (actions) expression builder.
    /// </summary>
    public interface IRightHandSideExpression
    {
        /// <summary>
        /// Defines rule's action that engine executes when the rule fires.
        /// </summary>
        /// <param name="action">Action expression.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Do(Expression<Action<IContext>> action);

        /// <summary>
        /// Defines rule's action that yields a linked fact when the rule fires.
        /// If the rule is fired due to an update, the linked fact is also updated with the new yielded value.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to yield.</typeparam>
        /// <param name="yield">Action expression that yields the linked fact.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yield);

        /// <summary>
        /// Defines rule's action that yields a linked fact when the rule fires.
        /// If the rule is fired due to an update, the update expression is evaluated to produce an updated linked fact.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to yield.</typeparam>
        /// <param name="yieldInsert">Action expression that yields a new linked fact if the linked fact does not yet exist.</param>
        /// <param name="yieldUpdate">Action expression that yields an updated linked fact if the linked fact already exists.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yieldInsert, Expression<Func<IContext, TFact, TFact>> yieldUpdate);
    }
}