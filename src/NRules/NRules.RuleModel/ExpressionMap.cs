using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel
{
    /// <summary>
    /// Sorted readonly map of named expressions.
    /// </summary>
    public class ExpressionMap : IEnumerable<NamedExpressionElement>
    {
        private readonly SortedDictionary<string, NamedExpressionElement> _expressions;

        public ExpressionMap(IEnumerable<NamedExpressionElement> expressions)
        {
            _expressions = new SortedDictionary<string, NamedExpressionElement>(expressions.ToDictionary(x => x.Name));
        }

        /// <summary>
        /// Number of expressions in the map.
        /// </summary>
        public int Count => _expressions.Count;

        /// <summary>
        /// Retrieves expression by name.
        /// </summary>
        /// <param name="name">Expression name.</param>
        /// <returns>Matching expression.</returns>
        public NamedExpressionElement this[string name]
        {
            get
            {
                var found = _expressions.TryGetValue(name, out var result);
                if (!found)
                {
                    throw new ArgumentException(
                        $"Expression with the given name not found. Name={name}", nameof(name));
                }
                return result;
            }
        }

        public IEnumerator<NamedExpressionElement> GetEnumerator()
        {
            return _expressions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}