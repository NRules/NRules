using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IBetaMemoryNode : ITupleSource, INode
    {
        IEnumerable<ITupleSink> Sinks { get; }
        void PropagateAssert(IExecutionContext context, ITupleFactList tupleFactList);
        void PropagateUpdate(IExecutionContext context, ITupleFactList tupleFactList);
        void PropagateRetract(IExecutionContext context, ITupleFactList tupleFactList);
    }

    internal class BetaMemoryNode : IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public IEnumerable<ITupleSink> Sinks { get { return _sinks; } }

        public void PropagateAssert(IExecutionContext context, ITupleFactList tupleFactList)
        {
            if (tupleFactList.Count == 0) return;
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toAssert = new List<Tuple>();
            var enumerator = tupleFactList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var childTuple = new Tuple(enumerator.CurrentTuple, enumerator.CurrentFact);
                childTuple.GroupId = enumerator.CurrentTuple.Id;
                toAssert.Add(childTuple);
            }

            PropagateAssertInternal(context, memory, toAssert);
        }

        public void PropagateUpdate(IExecutionContext context, ITupleFactList tupleFactList)
        {
            if (tupleFactList.Count == 0) return;
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toAssert = new List<Tuple>();
            var toUpdate = new List<Tuple>();
            var enumerator = tupleFactList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Tuple childTuple = memory.FindTuple(enumerator.CurrentTuple, enumerator.CurrentFact);
                if (childTuple == null)
                {
                    childTuple = new Tuple(enumerator.CurrentTuple, enumerator.CurrentFact);
                    childTuple.GroupId = enumerator.CurrentTuple.Id;
                    toAssert.Add(childTuple);
                }
                else
                {
                    toUpdate.Add(childTuple);
                }
            }

            PropagateAssertInternal(context, memory, toAssert);
            PropagateUpdateInternal(context, toUpdate);
        }

        public void PropagateRetract(IExecutionContext context, ITupleFactList tupleFactList)
        {
            if (tupleFactList.Count == 0) return;
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toRetract = new List<Tuple>();
            var enumerator = tupleFactList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Tuple childTuple = memory.FindTuple(enumerator.CurrentTuple, enumerator.CurrentFact);
                if (childTuple != null)
                {
                    toRetract.Add(childTuple);
                }
            }

            PropagateRetractInternal(context, memory, toRetract);
        }

        private void PropagateAssertInternal(IExecutionContext context, IBetaMemory memory, List<Tuple> tuples)
        {
            if (tuples.Count > 0)
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateAssert(context, tuples);
                }
                foreach (var childTuple in tuples)
                {
                    memory.Add(childTuple);
                }
            }
        }

        private void PropagateUpdateInternal(IExecutionContext context, List<Tuple> tuples)
        {
            if (tuples.Count > 0)
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateUpdate(context, tuples);
                }
            }
        }

        private void PropagateRetractInternal(IExecutionContext context, IBetaMemory memory, List<Tuple> tuples)
        {
            if (tuples.Count > 0)
            {
                foreach (var sink in _sinks)
                {
                    sink.PropagateRetract(context, tuples);
                }
                foreach (var childTuple in tuples)
                {
                    memory.Remove(childTuple);
                }
            }
        }

        public IEnumerable<Tuple> GetTuples(IExecutionContext context)
        {
            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            return memory.Tuples;
        }

        public void Attach(ITupleSink sink)
        {
            _sinks.Add(sink);
        }

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBetaMemoryNode(context, this);
        }
    }
}