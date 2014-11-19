using System;

namespace NRules.Events
{
    /// <summary>
    /// Information related to failure events that allows observer to mark error as handled.
    /// </summary>
    public class RecoverableErrorEventArgs : ErrorEventArgs
    {
        internal RecoverableErrorEventArgs(Exception exception) : base(exception)
        {
            IsHandled = false;
        }

        /// <summary>
        /// Flag indicating whether the exception was handled.
        /// If handler sets this to <code>true</code> then engine continues execution,
        /// otherwise exception is rethrown and terminates engine's execution.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}