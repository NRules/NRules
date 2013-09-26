using System;

namespace NRules.Rule
{
    public abstract class PatternSourceElement : RuleElement
    {
        public Type ResultType { get; private set; }

        internal PatternSourceElement(Type resultType)
        {
            ResultType = resultType;
        }
    }
}