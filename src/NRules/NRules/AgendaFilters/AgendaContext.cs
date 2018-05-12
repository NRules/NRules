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

        internal AgendaContext(ISessionInternal session, IEventAggregator eventAggregator)
        {
            Session = session;
            EventAggregator = eventAggregator;
        }
    }
}
