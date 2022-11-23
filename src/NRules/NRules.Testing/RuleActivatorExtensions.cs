using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Extension methods for <see cref="IRuleActivator"/>
/// </summary>
public static class RuleActivatorExtensions
{
    /// <summary>
    /// Converts <see cref="IRuleActivator"/> to <see cref="CachedRuleActivator"/>
    /// </summary>
    /// <param name="activator">Activator to convert</param>
    /// <returns>Returns new <see cref="CachedRuleActivator"/> if <paramref name="activator"/> is not <see cref="CachedRuleActivator"/>.</returns>
    public static CachedRuleActivator AsCached(this IRuleActivator activator) =>
        activator switch
        {
            CachedRuleActivator c => c,
            _ => new CachedRuleActivator(activator),
        };

    internal static IEnumerable<T> Activate<T>(this IRuleActivator activator)
        where T : Rule =>
        activator.Activate(typeof(T)).Cast<T>();
}
