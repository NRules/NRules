using System;
using System.Collections.Generic;
using NRules.Diagnostics;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class BindingNode : BetaNode
    {
        private readonly ILhsTupleExpression<object> _compiledExpression;

        public ExpressionElement ExpressionElement { get; }
        public Type ResultType { get; }
        public ITupleSource Source { get; }
        
        public BindingNode(ExpressionElement expressionElement, ILhsTupleExpression<object> compiledExpression, Type resultType, ITupleSource source)
        {
            ExpressionElement = expressionElement;
            _compiledExpression = compiledExpression;
            ResultType = resultType;
            Source = source;

            Source.Attach(this);
        }

        public override void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
        {
            var toAssert = new TupleFactList();
            using (var counter = PerfCounter.Assert(context, this))
            {
                foreach (var tuple in tuples)
                {
                    AssertBinding(context, tuple, toAssert);
                }

                counter.AddItems(tuples.Count);
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
        {
            var toAssert = new TupleFactList();
            var toUpdate = new TupleFactList();
            var toRetract = new TupleFactList();
            using (var counter = PerfCounter.Update(context, this))
            {
                foreach (var tuple in tuples)
                {
                    var fact = context.WorkingMemory.GetState<Fact>(this, tuple);
                    if (fact != null)
                    {
                        UpdateBinding(context, tuple, fact, toUpdate, toRetract);
                    }
                    else
                    {
                        AssertBinding(context, tuple, toAssert);
                    }
                }

                counter.AddItems(tuples.Count);
            }
            MemoryNode.PropagateRetract(context, toRetract);
            MemoryNode.PropagateUpdate(context, toUpdate);
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            using (var counter = PerfCounter.Retract(context, this))
            {
                foreach (var tuple in tuples)
                {
                    RetractBinding(context, tuple, toRetract);
                }

                counter.AddItems(tuples.Count);
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        private void AssertBinding(IExecutionContext context, Tuple tuple, TupleFactList toAssert)
        {
            try
            {
                var value = _compiledExpression.Invoke(context, NodeInfo, tuple);
                var fact = new Fact(value, ResultType);
                context.WorkingMemory.SetState(this, tuple, fact);
                toAssert.Add(tuple, fact);
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException("Failed to evaluate binding expression",
                        e.Expression.ToString(), e.InnerException);
                }
            }
        }

        private void UpdateBinding(IExecutionContext context, Tuple tuple, Fact fact, TupleFactList toUpdate, TupleFactList toRetract)
        {
            try
            {
                var value = _compiledExpression.Invoke(context, NodeInfo, tuple);
                fact.RawObject = value;
                toUpdate.Add(tuple, fact);
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException("Failed to evaluate binding expression",
                        e.Expression.ToString(), e.InnerException);
                }
                RetractBinding(context, tuple, toRetract);
            }
        }

        private void RetractBinding(IExecutionContext context, Tuple tuple, TupleFactList toRetract)
        {
            var fact = context.WorkingMemory.RemoveState<Fact>(this, tuple);
            if (fact != null)
            {
                toRetract.Add(tuple, fact);
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBindingNode(context, this);
        }
    }
}
