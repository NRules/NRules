using System;
using System.Linq.Expressions;

namespace NRules.Dsl
{
    /// <summary>
    /// Rule's right hand side (actions) expression builder.
    /// </summary>
    public interface IRightHandSide
    {
        /// <summary>
        /// Defines a new rule's action that engine executes when the rule fires.
        /// </summary>
        /// <param name="action">Action expression.</param>
        /// <returns>Right hand side expression builder.</returns>
        IRightHandSide Do(Expression<Action<IContext>> action);
    }
}