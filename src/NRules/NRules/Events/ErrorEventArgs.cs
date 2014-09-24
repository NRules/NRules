using System;
using NRules.Exceptions;

namespace NRules.Events
{
    public class ErrorEventArgs<TException> : EventArgs where TException: ExecutionException
    {
        public TException Exception { get; private set; }

        internal ErrorEventArgs(TException exception)
        {
            Exception = exception;
            IsHandled = false;
        }

        public bool IsHandled { get; set; }
    }
}