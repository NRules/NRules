using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel
{
    /// <summary>
    /// Ordered readonly collection of named expressions.
    /// </summary>
    public class ExpressionCollection : IEnumerable<NamedExpressionElement>
    {
        private readonly List<NamedExpressionElement> _expressions;

        internal ExpressionCollection(IEnumerable<NamedExpressionElement> expressions)
        {
            _expressions = new List<NamedExpressionElement>(expressions);
        }

        /// <summary>
        /// Number of expressions in the collection.
        /// </summary>
        public int Count => _expressions.Count;

        /// <summary>
        /// Retrieves single expression by name.
        /// </summary>
        /// <param name="name">Expression name.</param>
        /// <returns>Matching expression.</returns>
        public NamedExpressionElement this[string name]
        {
            get
            {
                var result = FindSingleOrDefault(name);
                if (result == null)
                {
                    throw new ArgumentException(
                        $"Expression with the given name not found. Name={name}", nameof(name));
                }
                return result;
            }
        }

        /// <summary>
        /// Retrieves expressions by name.
        /// </summary>
        /// <param name="name">Expression name.</param>
        /// <returns>Matching expression or empty IEnumerable.</returns>
        public IEnumerable<NamedExpressionElement> Find(string name)
        {
            return _expressions.Where(e => e.Name == name);
        }

        /// <summary>
        /// Retrieves single expression by name.
        /// </summary>
        /// <param name="name">Expression name.</param>
        /// <returns>Matching expression or <c>null</c>.</returns>
        public NamedExpressionElement FindSingleOrDefault(string name)
        {
            return Find(name).SingleOrDefault();
        }

        public IEnumerator<NamedExpressionElement> GetEnumerator()
        {
            return _expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}