using NRules.Diagnostics;

namespace NRules
{
    internal interface IExecutionContext
    {
        IWorkingMemory WorkingMemory { get; }
        IAgenda Agenda { get; }
        IEventAggregator EventAggregator { get; }
    }

    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(IWorkingMemory workingMemory, IAgenda agenda, IEventAggregator eventAggregator)
        {
            WorkingMemory = workingMemory;
            Agenda = agenda;
            EventAggregator = eventAggregator;
        }

        public IWorkingMemory WorkingMemory { get; private set; }
        public IAgenda Agenda { get; private set; }
        public IEventAggregator EventAggregator { get; private set; }
    }
}