using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete
{
    internal class JoinNode : BinaryBetaNode
    {
        private readonly bool _isSubnetJoin;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<IBetaCondition> Conditions { get; }

        public JoinNode(ITupleSource leftSource, IObjectSource rightSource, bool isSubnetJoin)
            : base(leftSource, rightSource)
        {
            _isSubnetJoin = isSubnetJoin;
            Conditions = new List<IBetaCondition>();
        }

        public override void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
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

        public override void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
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
            MemoryNode.PropagateRetract(context, toRetract);
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
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

        public override void PropagateAssert(IExecutionContext context, List<Fact> facts)
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

        public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
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
            MemoryNode.PropagateRetract(context, toRetract);
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
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

        private bool MatchesConditions(IExecutionContext context, Tuple left, Fact right)
        {
            foreach (var condition in Conditions)
            {
                if (!condition.IsSatisfiedBy(context, NodeInfo, left, right))
                    return false;
            }
            return true;
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitJoinNode(context, this);
        }
    }
}