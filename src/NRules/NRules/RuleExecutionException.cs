using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur during rules execution.
    /// </summary>
    [Serializable]
    public class RuleExecutionException : Exception
    {
        internal RuleExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [System.Security.SecuritySafeCritical]
        protected RuleExecutionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}