using System;

namespace NRules.Exceptions
{
    public class ExecutionException : Exception
    {
        internal ExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}