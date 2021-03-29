using System;
using System.Linq.Expressions;

namespace NRules.Extensibility
{
    /// <summary>
    /// Compiles expressions used in rules conditions and actions in a form of expression trees
    /// into executable delegates.
    /// The default implementation uses built-in .NET expression compiler.
    /// </summary>
    public interface IExpressionCompiler
    {
        /// <summary>
        /// Compiles an expression tree into an executable delegate.
        /// </summary>
        /// <typeparam name="TDelegate">Type of the underlying expression delegate.</typeparam>
        /// <param name="expression">Expression tree to compile.</param>
        /// <returns>The compiled delegate.</returns>
        TDelegate Compile<TDelegate>(Expression<TDelegate> expression) where TDelegate : Delegate;
    }

    internal class ExpressionCompiler : IExpressionCompiler
    {
        public TDelegate Compile<TDelegate>(Expression<TDelegate> expression) where TDelegate : Delegate
        {
            return expression.Compile();
        }
    }
}
