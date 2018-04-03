using System.Collections.Generic;
using System.Linq;

namespace NRules.Utilities
{
    internal static class IndexMapExtensions
    {
        /// <summary>
        /// Converts sequence to an index lookup map.
        /// </summary>
        /// <typeparam name="TElement">Type of element in the sequence.</typeparam>
        /// <param name="sequence">Sequence to convert.</param>
        /// <returns>Index lookup dictionary.</returns>
        public static Dictionary<TElement, int> ToIndexMap<TElement>(this IEnumerable<TElement> sequence)
        {
            return sequence.Select((value, index) => new {value, index}).ToDictionary(x => x.value, x => x.index);
        }

        /// <summary>
        /// Returns element's index or -1.
        /// </summary>
        /// <typeparam name="TElement">Type of element in the index map.</typeparam>
        /// <param name="indexMap">Index map.</param>
        /// <param name="element">Element to lookup.</param>
        /// <returns></returns>
        public static int IndexOrDefault<TElement>(this Dictionary<TElement, int> indexMap, TElement element)
        {
            if (indexMap.TryGetValue(element, out var index))
            {
                return index;
            }
            return -1;
        }
    }
}