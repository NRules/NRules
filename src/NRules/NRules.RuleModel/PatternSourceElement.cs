using System;
using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that serves as a source to pattern elements.
    /// </summary>
    public abstract class PatternSourceElement : RuleLeftElement
    {
        /// <summary>
        /// Type of the result that this rule element yields.
        /// </summary>
        public Type ResultType { get; }

        internal PatternSourceElement(IEnumerable<Declaration> declarations, Type resultType)
            : base(declarations)
        {
            ResultType = resultType;
        }
    }
}