using System.Collections.Generic;
using NRules.Diagnostics;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class JoinNode : BinaryBetaNode
    {
        private readonly List<ILhsExpression<bool>> _compiledExpressions;
        private readonly bool _isSubnetJoin;

        public List<Declaration> Declarations { get; }
        public List<ExpressionElement> ExpressionElements { get; }

        public JoinNode(ITupleSource leftSource, 
            IObjectSource rightSource,
            List<Declaration> declarations,
            List<ExpressionElement> expressionElements,
            List<ILhsExpression<bool>> compiledExpressions,
            bool isSubnetJoin)
            : base(leftSource, rightSource, isSubnetJoin)
        {
            Declarations = declarations;
            ExpressionElements = expressionElements;
            _compiledExpressions = compiledExpressions;
            _isSubnetJoin = isSubnetJoin;
        }

        public override void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            var toAssert = new TupleFactList();
            using (var counter = PerfCounter.Assert(context, this))
            {
                var joinedSets = JoinedSets(context, tuples);
                foreach (var set in joinedSets)
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                    {
                        toAssert.Add(set.Tuple, fact);
                    }
                }

                counter.AddInputs(tuples.Count);
                counter.AddOutputs(toAssert.Count);
            }

            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            if (_isSubnetJoin) return;
            
            var toUpdate = new TupleFactList();
            var toRetract = new TupleFactList();
            using (var counter = PerfCounter.Update(context, this))
            {
                var joinedSets = JoinedSets(context, tuples);
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

                counter.AddInputs(tuples.Count);
                counter.AddOutputs(toUpdate.Count + toRetract.Count);
            }

            MemoryNode.PropagateRetract(context, toRetract);
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            using (var counter = PerfCounter.Retract(context, this))
            {
                var joinedSets = JoinedSets(context, tuples);
                foreach (var set in joinedSets)
                foreach (var fact in set.Facts)
                {
                    toRetract.Add(set.Tuple, fact);
                }
             
                counter.AddInputs(tuples.Count);
                counter.AddOutputs(toRetract.Count);
            }

            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void PropagateAssert(IExecutionContext context, List<Fact> facts)
        {
            var toAssert = new TupleFactList();
            using (var counter = PerfCounter.Assert(context, this))
            {
                var joinedSets = JoinedSets(context, facts);
                foreach (var set in joinedSets)
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                    {
                        toAssert.Add(set.Tuple, fact);
                    }
                }

                counter.AddInputs(facts.Count);
                counter.AddOutputs(toAssert.Count);
            }

            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            var toUpdate = new TupleFactList();
            var toRetract = new TupleFactList();
            using (var counter = PerfCounter.Update(context, this))
            {
                var joinedSets = JoinedSets(context, facts);
                foreach (var set in joinedSets)
                foreach (var fact in set.Facts)
                {
                    if (MatchesConditions(context, set.Tuple, fact))
                        toUpdate.Add(set.Tuple, fact);
                    else
                        toRetract.Add(set.Tuple, fact);
                }

                counter.AddInputs(facts.Count);
                counter.AddOutputs(toUpdate.Count + toRetract.Count);
            }

            MemoryNode.PropagateRetract(context, toRetract);
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            var toRetract = new TupleFactList();
            using (var counter = PerfCounter.Retract(context, this))
            {
                var joinedSets = JoinedSets(context, facts);
                foreach (var set in joinedSets)
                foreach (var fact in set.Facts)
                {
                    toRetract.Add(set.Tuple, fact);
                }

                counter.AddInputs(facts.Count);
                counter.AddOutputs(toRetract.Count);
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