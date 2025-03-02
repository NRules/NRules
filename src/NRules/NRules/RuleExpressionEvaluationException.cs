using System;
using System.Runtime.Serialization;
using System.Security;

namespace NRules;

/// <summary>
/// Represents errors that occur while evaluating expressions as part of rules execution.
/// </summary>
[Serializable]
public abstract class RuleExpressionEvaluationException : RuleExecutionException
{
    private protected RuleExpressionEvaluationException(string message, string expression, Exception innerException)
        : base(message, innerException)
    {
        Expression = expression;
    }

    [SecuritySafeCritical]
    protected RuleExpressionEvaluationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Expression = info.GetString("Expression");
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }
        base.GetObjectData(info, context);
        info.AddValue("Expression", Expression, typeof(string));
    }

    /// <summary>
    /// Expression that caused exception.
    /// </summary>
    public string Expression { get; }

    public override string Message
    {
        get
        {
            string message = base.Message;
            if (!string.IsNullOrEmpty(Expression))
            {
                return message + Environment.NewLine + Expression;
            }
            return message;
        }
    } 
}