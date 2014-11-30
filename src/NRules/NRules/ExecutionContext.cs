using NRules.Diagnostics;

namespace NRules
{
    internal interface IExecutionContext
    {
        ISession Session { get; }
        IWorkingMemory WorkingMemory { get; }
        IAgenda Agenda { get; }
        IEventAggregator EventAggregator { get; }
    }

    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(ISession session, IWorkingMemory workingMemory, IAgenda agenda, IEventAggregator eventAggregator)
        {
            Session = session;
            WorkingMemory = workingMemory;
            Agenda = agenda;
            EventAggregator = eventAggregator;
        }

        public ISession Session { get; private set; }
        public IWorkingMemory WorkingMemory { get; private set; }
        public IAgenda Agenda { get; private set; }
        public IEventAggregator EventAggregator { get; private set; }
    }
}