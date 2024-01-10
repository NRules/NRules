using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

internal static class EnumerableExtensions
{
    public static T[] AsArray<T>(this IReadOnlyCollection<T> source)
    {
        return source as T[] ?? source.ToArray();
    }
}