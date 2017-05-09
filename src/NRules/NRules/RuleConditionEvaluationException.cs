using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule condition.
    /// </summary>
#if NET45
    [Serializable]
#endif
    public class RuleConditionEvaluationException : RuleExpressionEvaluationException
    {
        internal RuleConditionEvaluationException(string message, string expression, Exception innerException)
            : base(message, expression, innerException)
        {
        }

#if NET45
        [System.Security.SecuritySafeCritical]
        protected RuleConditionEvaluationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}