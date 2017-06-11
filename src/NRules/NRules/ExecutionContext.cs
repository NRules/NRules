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

        public ISessionInternal Session { get; }
        public IWorkingMemory WorkingMemory { get; }
        public IAgendaInternal Agenda { get; }
        public IEventAggregator EventAggregator { get; }
    }
}