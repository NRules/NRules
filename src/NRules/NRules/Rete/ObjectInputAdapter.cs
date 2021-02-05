using System.Collections.Generic;

namespace NRules.Rete
{
    internal class ObjectInputAdapter : ITupleSink, IAlphaMemoryNode
    {
        private readonly ITupleSource _source;
        private readonly List<IObjectSink> _sinks = new List<IObjectSink>();

        public IEnumerable<IObjectSink> Sinks => _sinks;

        public ObjectInputAdapter(IBetaMemoryNode source)
        {
            _source = source;
            source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            var toAssert = new List<Fact>(tuples.Count);
            foreach (var tuple in tuples)
            {
                var wrapperFact = new WrapperFact(tuple);
                context.WorkingMemory.SetState(this, tuple, wrapperFact);
                toAssert.Add(wrapperFact);
            }
            foreach (var sink in _sinks)
            {
                sink.PropagateAssert(context, toAssert);
            }
        }

        public void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            var toUpdate = new List<Fact>(tuples.Count);
            foreach (var tuple in tuples)
            {
                var wrapperFact = context.WorkingMemory.GetStateOrThrow<WrapperFact>(this, tuple);
                toUpdate.Add(wrapperFact);
            }
            foreach (var sink in _sinks)
            {
                sink.PropagateUpdate(context, toUpdate);
            }
        }

        public void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            var toRetract = new List<Fact>(tuples.Count);
            foreach (var tuple in tuples)
            {
                var wrapperFact = context.WorkingMemory.GetStateOrThrow<WrapperFact>(this, tuple);
                toRetract.Add(wrapperFact);
            }
            foreach (var sink in _sinks)
            {
                sink.PropagateRetract(context, toRetract);
            }
            foreach (var tuple in tuples)
            {
                context.WorkingMemory.RemoveStateOrThrow<WrapperFact>(this, tuple);
            }
        }

        public IEnumerable<Fact> GetFacts(IExecutionContext context)
        {
            var sourceTuples = _source.GetTuples(context);
            var sourceFacts = new List<Fact>();
            foreach (var sourceTuple in sourceTuples)
            {
                var wrapperFact = context.WorkingMemory.GetStateOrThrow<WrapperFact>(this, sourceTuple);
                sourceFacts.Add(wrapperFact);
            }
            return sourceFacts;
        }

        public void Attach(IObjectSink sink)
        {
            _sinks.Add(sink);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitObjectInputAdapter(context, this);
        }
    }
}