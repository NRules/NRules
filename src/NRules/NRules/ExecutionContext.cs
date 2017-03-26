using NRules.Diagnostics;

namespace NRules
{
    internal interface IExecutionContext
    {
        ISessionInternal Session { get; }
        IWorkingMemory WorkingMemory { get; }
        IAgendaInternal Agenda { get; }
        IEventAggregator EventAggregator { get; }
    }

    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(ISessionInternal session, IWorkingMemory workingMemory, IAgendaInternal agenda, IEventAggregator eventAggregator)
        {
            Session = session;
            WorkingMemory = workingMemory;
            Agenda = agenda;
            EventAggregator = eventAggregator;
        }

        public ISessionInternal Session { get; private set; }
        public IWorkingMemory WorkingMemory { get; private set; }
        public IAgendaInternal Agenda { get; private set; }
        public IEventAggregator EventAggregator { get; private set; }
    }
}