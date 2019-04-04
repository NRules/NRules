using System.Collections.Generic;

namespace NRules.Rete
{
    internal class ExistsNode : BinaryBetaNode
    {
        public ExistsNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource)
        {
        }

        public override void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            var joinedSets = JoinedSets(context, tuples);
            var toAssert = new TupleFactList();
            foreach (var set in joinedSets)
            {
                var quantifier = set.Tuple.CreateQuantifier(this);
                foreach (var fact in set.Facts)
                {
                    quantifier.Value++;
                }
                if (quantifier.Value > 0)
                {
                    toAssert.Add(set.Tuple, null);
                }
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            var toUpdate = new TupleFactList();
            foreach (var tuple in tuples)
            {
                if (tuple.GetQuantifier(this).Value > 0)
                {
                    toUpdate.Add(tuple, null);
                }
            }
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            foreach (var tuple in tuples)
            {
                if (tuple.RemoveQuantifier(this).Value > 0)
                {
                    toRetract.Add(tuple, null);
                }
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void PropagateAssert(IExecutionContext context, List<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var toAssert = new TupleFactList();
            foreach (var set in joinedSets)
            {
                var quantifier = set.Tuple.GetQuantifier(this);
                int startingCount = quantifier.Value;
                foreach (var fact in set.Facts)
                {
                    quantifier.Value++;
                }
                if (startingCount == 0 && quantifier.Value > 0)
                {
                    toAssert.Add(set.Tuple, null);
                }
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            //Do nothing
        }

        public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var toRetract = new TupleFactList();
            foreach (var set in joinedSets)
            {
                var quantifier = set.Tuple.GetQuantifier(this);
                int startingCount = quantifier.Value;
                foreach (var fact in set.Facts)
                {
                    quantifier.Value--;
                }
                if (startingCount > 0 && quantifier.Value == 0)
                {
                    toRetract.Add(set.Tuple, null);
                }
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitExistsNode(context, this);
        }
    }
}