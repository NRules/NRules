using System;

namespace NRules.Events
{
    /// <summary>
    /// Information related to failure events.
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    public class ErrorEventArgs<TException> : EventArgs where TException : RuleExecutionException
    {
        internal ErrorEventArgs(TException exception)
        {
            Exception = exception;
            IsHandled = false;
        }

        /// <summary>
        /// Exception related to the event.
        /// </summary>
        public TException Exception { get; private set; }

        /// <summary>
        /// Flag indicating whether the exception was handled.
        /// If handler sets this to <code>true</code> then engine continues execution,
        /// otherwise exception is rethrown and terminates engine's execution.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}