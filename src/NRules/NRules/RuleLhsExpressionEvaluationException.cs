using System;
using System.Runtime.Serialization;
using System.Security;

namespace NRules;

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

    [SecuritySafeCritical]
    protected RuleLhsExpressionEvaluationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}