using System;
using System.Runtime.Serialization;
using System.Security;

namespace NRules.Fluent;

/// <summary>
/// Represents errors that occur while building rule definition using fluent DSL.
/// </summary>
[Serializable]
public class RuleDefinitionException : Exception
{
    internal RuleDefinitionException(string message, Type ruleType, Exception innerException)
        : base(message, innerException)
    {
        RuleTypeName = ruleType.AssemblyQualifiedName;
    }

    [SecuritySafeCritical]
    protected RuleDefinitionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        RuleTypeName = info.GetString("RuleTypeName");
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }
        base.GetObjectData(info, context);
        info.AddValue("RuleTypeName", RuleTypeName, typeof(string));
    }

    /// <summary>
    /// Rule CLR type that caused exception.
    /// </summary>
    public Type RuleType => Type.GetType(RuleTypeName);

    /// <summary>
    /// Rule CLR type name that caused exception.
    /// </summary>
    public string RuleTypeName { get; }

    public override string Message
    {
        get
        {
            string message = base.Message + Environment.NewLine + RuleTypeName;
            return message;
        }
    }
}
