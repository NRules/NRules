using System;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur during rules execution.
    /// </summary>
    public class RuleExecutionException : Exception
    {
        internal RuleExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}