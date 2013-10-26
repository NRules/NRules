using System;

namespace NRules.Rule
{
    /// <summary>
    /// Rule element that serves as a source to pattern elements.
    /// </summary>
    public abstract class PatternSourceElement : RuleElement
    {
        /// <summary>
        /// Type of the result that this rule element yields.
        /// </summary>
        public Type ResultType { get; private set; }

        internal PatternSourceElement(Type resultType)
        {
            ResultType = resultType;
        }
    }
}