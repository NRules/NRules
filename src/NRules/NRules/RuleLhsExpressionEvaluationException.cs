using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule left-hand side expression.
    /// </summary>
    [Serializable]
    public class RuleLhsExpressionEvaluationException : RuleExpressionEvaluationException
    {
        internal RuleLhsExpressionEvaluationException(string message, string expression, Exception innerException)
            : base(message, expression, innerException)
        {
        }

        [System.Security.SecuritySafeCritical]
        protected RuleLhsExpressionEvaluationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}