using System;

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
    }
}