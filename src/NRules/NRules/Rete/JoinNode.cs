using System.Collections.Generic;
using System.Diagnostics;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class JoinNode : BinaryBetaNode
    {
        private readonly List<ExpressionElement> _expressionElements;
        private readonly List<ILhsExpression<bool>> _compiledExpressions;
        private readonly bool _isSubnetJoin;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IEnumerable<ExpressionElement> ExpressionElements => _expressionElements;

        public JoinNode(ITupleSource leftSource, IObjectSource rightSource,
            List<ExpressionElement> expressionElements,
            List<ILhsExpression<bool>> compiledExpressions,
            bool isSubnetJoin)
            : base(leftSource, rightSource)
        {
            _expressionElements = expressionElements;
            _compiledExpressions = compiledExpressions;
            _isSubnetJoin = isSubnetJoin;
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
            try
            {
                foreach (var expression in _compiledExpressions)
                {
                    if (!expression.Invoke(context, NodeInfo, left, right))
                        return false;
                }
                return true;
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException(
                        "Failed to evaluate condition", e.Expression.ToString(), e.InnerException);
                }

                return false;
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitJoinNode(context, this);
        }
    }
}