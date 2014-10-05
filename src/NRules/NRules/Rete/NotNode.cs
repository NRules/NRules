using System.Linq;

namespace NRules.Rete
{
    internal class NotNode : BetaNode
    {
        public NotNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource)
        {
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            var matchingFacts = MatchingFacts(context, tuple);
            tuple.Quantifier().Value = matchingFacts.Count();
            if (tuple.Quantifier().Value == 0)
            {
                AssertTuple(context, tuple);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            if (tuple.Quantifier().Value == 0)
            {
                UpdateTuple(context, tuple);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            if (tuple.Quantifier().Value == 0)
            {
                RetractTuple(context, tuple);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            var matchingTuples = MatchingTuples(context, fact);
            foreach (var tuple in matchingTuples)
            {
                tuple.Quantifier().Value++;
                if (tuple.Quantifier().Value == 1)
                {
                    RetractTuple(context, tuple);
                }
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            //Do nothing
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            var matchingTuples = MatchingTuples(context, fact);
            foreach (var tuple in matchingTuples)
            {
                tuple.Quantifier().Value--;
                if (tuple.Quantifier().Value == 0)
                {
                    AssertTuple(context, tuple);
                }
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitNotNode(context, this);
        }

        private void AssertTuple(IExecutionContext context, Tuple tuple)
        {
            MemoryNode.PropagateAssert(context, new WrapperTuple(tuple, this));
        }

        private void UpdateTuple(IExecutionContext context, Tuple tuple)
        {
            var childTuple = tuple.ChildTuples.SingleOrDefault(x => x.Node == this);
            if (childTuple != null)
            {
                MemoryNode.PropagateUpdate(context, childTuple);
            }
        }

        private void RetractTuple(IExecutionContext context, Tuple tuple)
        {
            var childTuple = tuple.ChildTuples.SingleOrDefault(x => x.Node == this);
            if (childTuple != null)
            {
                MemoryNode.PropagateRetract(context, childTuple);
                childTuple.Clear();
            }
        }
    }
}