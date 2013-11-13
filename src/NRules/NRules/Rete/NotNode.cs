using System.Linq;

namespace NRules.Rete
{
    internal class NotNode : BetaNode
    {
        public NotNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource)
        {
        }

        public override void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            if (!RightSource.GetFacts(workingMemory).Any())
            {
                Sink.PropagateAssert(workingMemory, tuple);
            }
        }

        public override void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            if (!RightSource.GetFacts(workingMemory).Any())
            {
                Sink.PropagateUpdate(workingMemory, tuple);
            }
        }

        public override void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            if (!RightSource.GetFacts(workingMemory).Any())
            {
                Sink.PropagateRetract(workingMemory, tuple);
            }
        }

        public override void PropagateAssert(IWorkingMemory workingMemory, Fact fact)
        {
            if (RightSource.GetFacts(workingMemory).Count() == 1)
            {
                var tuples = LeftSource.GetTuples(workingMemory);
                foreach (var tuple in tuples)
                {
                    Sink.PropagateRetract(workingMemory, tuple);
                }
            }
        }

        public override void PropagateUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            //Do nothing
        }

        public override void PropagateRetract(IWorkingMemory workingMemory, Fact fact)
        {
            if (!RightSource.GetFacts(workingMemory).Any())
            {
                var tuples = LeftSource.GetTuples(workingMemory);
                foreach (var tuple in tuples)
                {
                    Sink.PropagateAssert(workingMemory, tuple);
                }
            }
        }
    }
}