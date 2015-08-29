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
    }
}