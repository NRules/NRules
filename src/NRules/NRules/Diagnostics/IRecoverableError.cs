using System;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Error event that can be handled by the consumer.
    /// </summary>
    public interface IRecoverableError
    {
        /// <summary>
        /// Flag that indicates whether the exception was handled.
        /// If handler sets this to <c>true</c> then engine continues execution,
        /// otherwise exception is rethrown and terminates engine's execution.
        /// </summary>
        bool IsHandled { get; set; }

        /// <summary>
        /// Exception that caused the error.
        /// </summary>
        Exception Exception { get; }
    }
}