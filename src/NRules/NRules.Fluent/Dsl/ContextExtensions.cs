using System;
using NRules.RuleModel;

namespace NRules.Fluent.Dsl
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Updates existing fact in the rules engine's memory.
        /// First the update action is applied to the fact, then the fact is updated in the engine's memory.
        /// </summary>
        /// <param name="context">Context instance.</param>
        /// <param name="fact">Existing fact to update.</param>
        /// <param name="updateAction">Action to apply to the fact.</param>
        public static void Update<T>(this IContext context, T fact, Action<T> updateAction)
        {
            updateAction(fact);
            context.Update(fact);
        }

        /// <summary>
        /// Resolves a registered service (normally via an IoC container).
        /// </summary>
        /// <typeparam name="TService">Type of service to resolve.</typeparam>
        /// <param name="context">Context instance.</param>
        /// <returns>Service instance.</returns>
        public static TService Resolve<TService>(this IContext context)
        {
            var service = context.Resolve(typeof(TService));
            return (TService)service;
        }
    }
}