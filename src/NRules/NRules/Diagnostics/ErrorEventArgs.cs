using System;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to failure events.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception _exception;

        internal ErrorEventArgs(Exception exception)
        {
            _exception = exception;
        }

        /// <summary>
        /// Exception related to the event.
        /// </summary>
        public Exception Exception { get { return _exception; } }

        /// <summary>
        /// Flag that indicates whether the exception was handled.
        /// If handler sets this to <c>true</c> then engine continues execution,
        /// otherwise exception is rethrown and terminates engine's execution.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}