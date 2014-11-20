using System;
using System.Runtime.Serialization;
using System.Security;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule action.
    /// </summary>
    [Serializable]
    public class RuleActionEvaluationException : RuleExpressionEvaluationException
    {
        internal RuleActionEvaluationException(string message, string expression, Exception innerException)
            : base(message, expression, innerException)
        {
        }

        [SecuritySafeCritical]
        protected RuleActionEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}