using NRules.Exceptions;

namespace NRules.Events
{
    public class ActionErrorEventArgs : ErrorEventArgs<ActionEvaluationException>
    {
        internal ActionErrorEventArgs(ActionEvaluationException exception) : base(exception)
        {
        }
    }
}