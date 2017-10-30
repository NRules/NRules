using System;

namespace NRules.Diagnostics
{
    public class RecoverableErrorEventArgs : ErrorEventArgs
    {
        internal RecoverableErrorEventArgs(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Flag that indicates whether the exception was handled.
        /// If handler sets this to <c>true</c> then engine continues execution,
        /// otherwise exception is rethrown and terminates engine's execution.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}