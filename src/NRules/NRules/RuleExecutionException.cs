using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur during rules execution.
    /// </summary>
#if NET45
    [System.Serializable]
#endif
    public class RuleExecutionException : Exception
    {
        internal RuleExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET45
        [System.Security.SecuritySafeCritical]
        protected RuleExecutionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}