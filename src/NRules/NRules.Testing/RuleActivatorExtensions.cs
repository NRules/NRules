using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

public static class RuleActivatorExtensions
{
    public static IEnumerable<T> Activate<T>(this IRuleActivator activator)
        where T : Rule
    {
        return activator.Activate(typeof(T)).Cast<T>();
    }
}
