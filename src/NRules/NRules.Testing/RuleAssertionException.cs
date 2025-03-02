using System;
using System.Runtime.Serialization;
using System.Security;

namespace NRules.Testing;

/// <summary>
/// Exception that represents a violation of rule firing expectations.
/// </summary>
[Serializable]
public class RuleAssertionException : Exception
{
    /// <summary>
    /// Creates a new instance of the <see cref="RuleAssertionException"/>.
    /// </summary>
    public RuleAssertionException()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RuleAssertionException"/>
    /// with the specified message.
    /// </summary>
    /// <param name="message">Message that describes the reason of the assertion exception.</param>
    public RuleAssertionException(string message) : base(message)
    {
    }

    [SecuritySafeCritical]
    protected RuleAssertionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}