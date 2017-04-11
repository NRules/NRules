using System;
//using System.Runtime.Serialization;
using System.Security;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule condition.
    /// </summary>
    //[Serializable]
    public class RuleConditionEvaluationException : RuleExpressionEvaluationException
    {
        internal RuleConditionEvaluationException(string message, string expression, Exception innerException)
            : base(message, expression, innerException)
        {
        }

        /*[SecuritySafeCritical]
        protected RuleConditionEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }*/
    }
}