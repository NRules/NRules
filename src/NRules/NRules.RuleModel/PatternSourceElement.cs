using System;

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

        internal PatternSourceElement(Type resultType)
        {
            ResultType = resultType;
        }
    }
}