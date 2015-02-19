using System.Linq;

namespace NRules.Rete
{
    internal class ExistsNode : BetaNode
    {
        public ExistsNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource)
        {
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            var matchingFacts = MatchingFacts(context, tuple);
            tuple.Quantifier(this).Value = matchingFacts.Count();
            if (tuple.Quantifier(this).Value > 0)
            {
                AssertTuple(context, tuple);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            if (tuple.Quantifier(this).Value > 0)
            {
                UpdateTuple(context, tuple);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            if (tuple.Quantifier(this).Value > 0)
            {
                RetractTuple(context, tuple);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            var matchingTuples = MatchingTuples(context, fact);
            foreach (var tuple in matchingTuples)
            {
                tuple.Quantifier(this).Value++;
                if (tuple.Quantifier(this).Value == 1)
                {
                    AssertTuple(context, tuple);
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
                tuple.Quantifier(this).Value--;
                if (tuple.Quantifier(this).Value == 0)
                {
                    RetractTuple(context, tuple);
                }
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitExistsNode(context, this);
        }

        private void AssertTuple(IExecutionContext context, Tuple tuple)
        {
            MemoryNode.PropagateAssert(context, tuple, null);
        }

        private void UpdateTuple(IExecutionContext context, Tuple tuple)
        {
            MemoryNode.PropagateUpdate(context, tuple, null);
        }

        private void RetractTuple(IExecutionContext context, Tuple tuple)
        {
            MemoryNode.PropagateRetract(context, tuple, null);
        }
    }
}