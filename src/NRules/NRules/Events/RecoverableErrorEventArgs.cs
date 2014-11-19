using System;

namespace NRules.Events
{
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