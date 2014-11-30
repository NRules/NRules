using System;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to failure events.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        internal ErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// Exception related to the event.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}