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
        /// Defines rule's action that engine executes for a given trigger.
        /// </summary>
        /// <param name="action">Action expression.</param>
        /// <param name="actionTrigger">Events that should trigger this action.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Action(Expression<Action<IContext>> action, ActionTrigger actionTrigger);

        /// <summary>
        /// Defines rule's action that engine executes when the rule fires
        /// due to the initial rule match or due to an update.
        /// </summary>
        /// <param name="action">Action expression.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Do(Expression<Action<IContext>> action);

        /// <summary>
        /// Defines rule's action that engine executes when the rule fires
        /// due to the match removal (provided the rule previously fired on the match).
        /// </summary>
        /// <param name="action">Action expression.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Undo(Expression<Action<IContext>> action);

        /// <summary>
        /// Defines rule's action that yields a linked fact when the rule fires.
        /// If the rule fires due to an update, the linked fact is also updated with the new yielded value.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to yield.</typeparam>
        /// <param name="yield">Action expression that yields the linked fact.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yield);

        /// <summary>
        /// Defines rule's action that yields a linked fact when the rule fires.
        /// If the rule fires due to an update, the update expression is evaluated to produce an updated linked fact.
        /// </summary>
        /// <typeparam name="TFact">Type of fact to yield.</typeparam>
        /// <param name="yieldInsert">Action expression that yields a new linked fact.</param>
        /// <param name="yieldUpdate">Action expression that yields an updated linked fact.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSideExpression Yield<TFact>(Expression<Func<IContext, TFact>> yieldInsert, Expression<Func<IContext, TFact, TFact>> yieldUpdate);
    }
}