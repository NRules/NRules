using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule condition.
    /// </summary>
    [Serializable]
    public class RuleConditionEvaluationException : RuleExpressionEvaluationException
    {
        internal RuleConditionEvaluationException(string message, string expression, Exception innerException)
            : base(message, expression, innerException)
        {
        }
    }
}