using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Fluent
{
    internal static class EnumerableExtensions
    {
        public static TValue? SingleNullable<T, TValue>(this IEnumerable<T> sequence, Func<T, TValue> projection)
            where T : Attribute where TValue : struct
        {
            T element = sequence.SingleOrDefault();
            if (element == null)
            {
                return null;
            }
            return projection(element);
        }
    }
}