using System;

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

    [System.Security.SecuritySafeCritical]
    protected RuleRhsExpressionEvaluationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
        RuleName = info.GetString("RuleName");
    }

    [System.Security.SecurityCritical]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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