using System;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    /// <summary>
    /// Rule's dependencies expression builder.
    /// </summary>
    public interface IDependencyExpression
    {
        /// <summary>
        /// Configures the engine to inject the rules with a required dependency. 
        /// </summary>
        /// <typeparam name="TDependency">Type of the service to inject.</typeparam>
        /// <param name="alias">Alias for the injected service.</param>
        /// <returns>Dependencies expression builder.</returns>
        IDependencyExpression Resolve<TDependency>(Expression<Func<TDependency>> alias);
    }
}