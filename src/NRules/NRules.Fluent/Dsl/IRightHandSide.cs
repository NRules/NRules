using System;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's right hand side (actions) expression builder.
    /// </summary>
    public interface IRightHandSide
    {
        /// <summary>
        /// Defines rule's action that engine executes when the rule fires.
        /// </summary>
        /// <param name="action">Action expression.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSide Do(Expression<Action<IContext>> action);
    }
}