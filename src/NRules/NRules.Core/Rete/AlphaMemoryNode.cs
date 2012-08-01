using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface IAlphaMemoryNode
    {
        void Attach(IObjectSink sink);
    }

    internal class AlphaMemoryNode : IObjectSink, IAlphaMemoryNode
    {
        private readonly List<IObjectSink> _sinks = new List<IObjectSink>();

        public Type FactType { get; private set; }

        public AlphaMemoryNode(Type factType)
        {
            FactType = factType;
        }

        public void PropagateAssert(IWorkingMemory workingMemory, Fact fact)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Facts.Add(fact);
            _sinks.ForEach(s => s.PropagateAssert(workingMemory, fact));
        }

        public void PropagateUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(this);
            if (memory.Facts.Contains(fact))
            {
                _sinks.ForEach(s => s.PropagateUpdate(workingMemory, fact));
            }
            else
            {
                PropagateAssert(workingMemory, fact);
            }
        }

        public void PropagateRetract(IWorkingMemory workingMemory, Fact fact)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(this);
            memory.Facts.Remove(fact);
            _sinks.ForEach(s => s.PropagateRetract(workingMemory, fact));
        }

        public IEnumerable<Fact> GetFacts(IWorkingMemory workingMemory)
        {
            IAlphaMemory memory = workingMemory.GetNodeMemory(this);
            return memory.Facts;
        }

        public void Attach(IObjectSink sink)
        {
            _sinks.Add(sink);
        }
    }
}