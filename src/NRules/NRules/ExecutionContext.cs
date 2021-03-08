using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules
{
    internal interface IExecutionContext
    {
        ISessionInternal Session { get; }
        IWorkingMemory WorkingMemory { get; }
        IAgendaInternal Agenda { get; }
        IEventAggregator EventAggregator { get; }
        IMetricsAggregator MetricsAggregator { get; }
        IIdGenerator IdGenerator { get; }
    }

    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(ISessionInternal session, 
            IWorkingMemory workingMemory, 
            IAgendaInternal agenda, 
            IEventAggregator eventAggregator, 
            IMetricsAggregator metricsAggregator, 
            IIdGenerator idGenerator)
        {
            Session = session;
            WorkingMemory = workingMemory;
            Agenda = agenda;
            EventAggregator = eventAggregator;
            MetricsAggregator = metricsAggregator;
            IdGenerator = idGenerator;
            UnlinkQueue = new Queue<Activation>();
        }

        public ISessionInternal Session { get; }
        public IWorkingMemory WorkingMemory { get; }
        public IAgendaInternal Agenda { get; }
        public IEventAggregator EventAggregator { get; }
        public IMetricsAggregator MetricsAggregator { get; }
        public IIdGenerator IdGenerator { get; }
        public Queue<Activation> UnlinkQueue { get; }
    }
}