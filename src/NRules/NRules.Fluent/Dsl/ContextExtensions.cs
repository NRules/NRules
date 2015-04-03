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
    }
}