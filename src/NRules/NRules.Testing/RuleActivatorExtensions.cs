using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

internal static class RuleActivatorExtensions
{
    internal static IEnumerable<T> Activate<T>(this IRuleActivator activator)
        where T : Rule =>
        activator.Activate(typeof(T)).Cast<T>();
}
