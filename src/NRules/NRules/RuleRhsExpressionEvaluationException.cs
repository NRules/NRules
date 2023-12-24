using System;
using System.Runtime.Serialization;
using System.Security;

namespace NRules;

/// <summary>
/// Represents errors that occur while evaluating rule right-hand side expression.
/// </summary>
[Serializable]
public class RuleRhsExpressionEvaluationException : RuleExpressionEvaluationException
{
    internal RuleRhsExpressionEvaluationException(string message, string ruleName, string expression, Exception innerException)
        : base(message, expression, innerException)
    {
        RuleName = ruleName;
    }

    [SecuritySafeCritical]
    protected RuleRhsExpressionEvaluationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        RuleName = info.GetString("RuleName");
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }
        base.GetObjectData(info, context);
        info.AddValue("RuleName", RuleName, typeof(string));
    }

    /// <summary>
    /// Rule that caused exception.
    /// </summary>
    public string RuleName { get; }

    public override string Message
    {
        get
        {
            string message = base.Message;
            if (!string.IsNullOrEmpty(RuleName))
            {
                return message + Environment.NewLine + RuleName;
            }
            return message;
        }
    }
}