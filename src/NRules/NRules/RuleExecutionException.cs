using System;
//using System.Runtime.Serialization;
using System.Security;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur during rules execution.
    /// </summary>
    //[Serializable]
    public class RuleExecutionException : Exception
    {
        internal RuleExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /*[SecuritySafeCritical]
        protected RuleExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }*/
    }
}