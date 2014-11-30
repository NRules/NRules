using System;
using NRules.Rete;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Fact in the working memory.
    /// </summary>
    public class FactInfo
    {
        private readonly Fact _fact;

        internal FactInfo(Fact fact)
        {
            _fact = fact;
        }

        /// <summary>
        /// Fact type.
        /// </summary>
        public Type Type
        {
            get { return _fact.FactType; }
        }

        /// <summary>
        /// Actual fact object.
        /// </summary>
        public object Value
        {
            get { return _fact.Object; }
        }
    }
}