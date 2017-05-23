using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Sorted readonly map of named expressions.
    /// </summary>
    public class ExpressionMap : IEnumerable<NamedExpression>
    {
        private readonly SortedDictionary<string, NamedExpression> _expressions;

        public ExpressionMap(IEnumerable<NamedExpression> expressions)
        {
            _expressions = new SortedDictionary<string, NamedExpression>(expressions.ToDictionary(x => x.Name));
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
        public LambdaExpression this[string name]
        {
            get
            {
                NamedExpression result;
                var found = _expressions.TryGetValue(name, out result);
                if (!found)
                {
                    throw new ArgumentException(
                        $"Expression with the given name not found. Name={name}", nameof(name));
                }
                return result.Expression;
            }
        }

        public IEnumerator<NamedExpression> GetEnumerator()
        {
            return _expressions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}