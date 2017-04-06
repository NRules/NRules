using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IAlphaMemoryNode : IObjectSource
    {
        IEnumerable<IObjectSink> Sinks { get; }
    }

    internal class AlphaMemoryNode : IObjectSink, IAlphaMemoryNode
    {
        private readonly List<IObjectSink> _sinks = new List<IObjectSink>();

        public IEnumerable<IObjectSink> Sinks { get { return _sinks; } }

        public void PropagateAssert(IExecutionContext context, IList<Fact> facts)
        {
            IAlphaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            foreach (var sink in _sinks)
            {
                sink.PropagateAssert(context, facts);
            }
            memory.Add(facts);
        }

        public void PropagateUpdate(IExecutionContext context, IList<Fact> facts)
        {
            IAlphaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toUpdate = new List<Fact>();
            var toAssert = new List<Fact>();
            foreach (var fact in facts)
            {
                if (memory.Contains(fact))
                    toUpdate.Add(fact);
                else
                    toAssert.Add(fact);
            }
            if (toUpdate.Count > 0)
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateUpdate(context, toUpdate);
                }
            }
            if (toAssert.Count > 0)
            {
                PropagateAssert(context, toAssert);
            }
        }

        public void PropagateRetract(IExecutionContext context, IList<Fact> facts)
        {
            IAlphaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toRetract = new List<Fact>(facts.Count);
            foreach (var fact in facts)
            {
                if (memory.Contains(fact))
                    toRetract.Add(fact);
            }
            if (toRetract.Count > 0)
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateRetract(context, toRetract);
                }
                memory.Remove(toRetract);
            }
        }

        public IEnumerable<Fact> GetFacts(IExecutionContext context)
        {
            IAlphaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            return memory.Facts;
        }

        public void Attach(IObjectSink sink)
        {
            _sinks.Add(sink);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitAlphaMemoryNode(context, this);
        }
    }
}