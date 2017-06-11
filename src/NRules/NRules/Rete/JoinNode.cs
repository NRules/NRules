using System.Collections.Generic;

namespace NRules.Rete
{
    internal class JoinNode : BetaNode
    {
        private readonly bool _isSubnetJoin;

        public JoinNode(ITupleSource leftSource, IObjectSource rightSource, bool isSubnetJoin)
            : base(leftSource, rightSource)
        {
            _isSubnetJoin = isSubnetJoin;
        }

        public override void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
        {
            var joinedSets = JoinedSets(context, tuples);
            var toAssert = new TupleFactList();
            foreach (var set in joinedSets)
            foreach (var fact in set.Facts)
            {
                if (MatchesConditions(context, set.Tuple, fact))
                {
                    toAssert.Add(set.Tuple, fact);
                }
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            if (_isSubnetJoin) return;

            var joinedSets = JoinedSets(context, tuples);
            var toUpdate = new TupleFactList();
            var toRetract = new TupleFactList();
            foreach (var set in joinedSets)
            foreach (var fact in set.Facts)
            {
                if (MatchesConditions(context, set.Tuple, fact))
                {
                    toUpdate.Add(set.Tuple, fact);
                }
                else
                {
                    toRetract.Add(set.Tuple, fact);
                }
            }
            MemoryNode.PropagateUpdate(context, toUpdate);
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var joinedSets = JoinedSets(context, tuples);
            var toRetract = new TupleFactList();
            foreach (var set in joinedSets)
            foreach (var fact in set.Facts)
            {
                toRetract.Add(set.Tuple, fact);
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void PropagateAssert(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var toAssert = new TupleFactList();
            foreach (var set in joinedSets)
            foreach (var fact in set.Facts)
            {
                if (MatchesConditions(context, set.Tuple, fact))
                {
                    toAssert.Add(set.Tuple, fact);
                }
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var toUpdate = new TupleFactList();
            var toRetract = new TupleFactList();
            foreach (var set in joinedSets)
            foreach (var fact in set.Facts)
            {
                if (MatchesConditions(context, set.Tuple, fact))
                    toUpdate.Add(set.Tuple, fact);
                else
                    toRetract.Add(set.Tuple, fact);
            }
            MemoryNode.PropagateUpdate(context, toUpdate);
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Fact> facts)
        {
            var joinedSets = JoinedSets(context, facts);
            var toRetract = new TupleFactList();
            foreach (var set in joinedSets)
            foreach (var fact in set.Facts)
            {
                toRetract.Add(set.Tuple, fact);
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitJoinNode(context, this);
        }
    }
}