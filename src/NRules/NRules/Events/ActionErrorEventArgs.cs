namespace NRules.Events
{
    /// <summary>
    /// Information related to error events raised during action execution.
    /// </summary>
    public class ActionErrorEventArgs : ErrorEventArgs<RuleActionEvaluationException>
    {
        internal ActionErrorEventArgs(RuleActionEvaluationException exception) : base(exception)
        {
        }
    }
}