using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete
{
    internal interface IBetaMemoryNode : ITupleSource, INode
    {
        IEnumerable<ITupleSink> Sinks { get; }
        void PropagateAssert(IExecutionContext context, TupleFactList tupleFactList);
        void PropagateUpdate(IExecutionContext context, TupleFactList tupleFactList);
        void PropagateRetract(IExecutionContext context, TupleFactList tupleFactList);
    }

    internal class BetaMemoryNode : IBetaMemoryNode
    {
        private readonly List<ITupleSink> _sinks = new List<ITupleSink>();

        public int Id { get; set; }
        public NodeInfo NodeInfo { get; } = new NodeInfo();
        public IEnumerable<ITupleSink> Sinks => _sinks;

        public void PropagateAssert(IExecutionContext context, TupleFactList tupleFactList)
        {
            if (tupleFactList.Count == 0) return;

            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toAssert = new List<Tuple>();

            using (var counter = PerfCounter.Assert(context, this))
            {
                var enumerator = tupleFactList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var childTuple = new Tuple(context.IdGenerator.NextTupleId(), enumerator.CurrentTuple,
                        enumerator.CurrentFact);
                    childTuple.GroupId = enumerator.CurrentTuple.Id;
                    toAssert.Add(childTuple);
                }
                
                counter.AddItems(tupleFactList.Count);
            }

            PropagateAssertInternal(context, memory, toAssert);
        }

        public void PropagateUpdate(IExecutionContext context, TupleFactList tupleFactList)
        {
            if (tupleFactList.Count == 0) return;

            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toAssert = new List<Tuple>();
            var toUpdate = new List<Tuple>();

            using (var counter = PerfCounter.Update(context, this))
            {
                var enumerator = tupleFactList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Tuple childTuple = memory.FindTuple(enumerator.CurrentTuple, enumerator.CurrentFact);
                    if (childTuple == null)
                    {
                        childTuple = new Tuple(context.IdGenerator.NextTupleId(), enumerator.CurrentTuple, enumerator.CurrentFact);
                        childTuple.GroupId = enumerator.CurrentTuple.Id;
                        toAssert.Add(childTuple);
                    }
                    else
                    {
                        toUpdate.Add(childTuple);
                    }
                }
                
                counter.AddItems(tupleFactList.Count);
            }

            PropagateAssertInternal(context, memory, toAssert);
            PropagateUpdateInternal(context, toUpdate);
        }

        public void PropagateRetract(IExecutionContext context, TupleFactList tupleFactList)
        {
            if (tupleFactList.Count == 0) return;

            IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
            var toRetract = new List<Tuple>();

            using (var counter = PerfCounter.Retract(context, this))
            {
                var enumerator = tupleFactList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Tuple childTuple = memory.FindTuple(enumerator.CurrentTuple, enumerator.CurrentFact);
                    if (childTuple != null)
                    {
                        toRetract.Add(childTuple);
                    }
                }
                
                counter.AddInputs(tupleFactList.Count);
                counter.AddOutputs(toRetract.Count);
            }

            PropagateRetractInternal(context, memory, toRetract);
        }

        private void PropagateAssertInternal(IExecutionContext context, IBetaMemory memory, List<Tuple> tuples)
        {
            if (tuples.Count > 0)
            {
                for (int i = 0; i < _sinks.Count; i++)
                {
                    _sinks[i].PropagateAssert(context, tuples);
                }

                using (var counter = PerfCounter.Assert(context, this))
                {
                    memory.Add(tuples);
                    counter.SetCount(memory.TupleCount);
                }
            }
        }

        private void PropagateUpdateInternal(IExecutionContext context, List<Tuple> tuples)
        {
            if (tuples.Count > 0)
            {
                for (int i = 0; i < _sinks.Count; i++)
                {
                    _sinks[i].PropagateUpdate(context, tuples);
                }
            }
        }

        private void PropagateRetractInternal(IExecutionContext context, IBetaMemory memory, List<Tuple> tuples)
        {
            if (tuples.Count > 0)
            {
                using (var counter = PerfCounter.Retract(context, this))
                {
                    memory.Remove(tuples);
                    counter.SetCount(memory.TupleCount);
                }

                for (int i = _sinks.Count - 1; i >= 0; i--)
                {
                    _sinks[i].PropagateRetract(context, tuples);
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