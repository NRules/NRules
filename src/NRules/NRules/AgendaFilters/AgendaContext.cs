using NRules.Diagnostics;

namespace NRules.AgendaFilters
{
    /// <summary>
    /// Context associated with the agenda operation.
    /// </summary>
    public class AgendaContext
    {
        internal ISessionInternal Session { get; }
        internal IEventAggregator EventAggregator { get; }

        internal AgendaContext(IExecutionContext context)
        {
            Session = context.Session;
            EventAggregator = context.EventAggregator;
        }
    }
}
