using System;
using System.Collections.Generic;

namespace NRules.Rete
{
    internal class BindingNode : BetaNode
    {
        public IBindingExpression BindingExpression { get; }
        public Type ResultType { get; }
        public ITupleSource Source { get; }

        public BindingNode(IBindingExpression bindingExpression, Type resultType, ITupleSource source)
        {
            BindingExpression = bindingExpression;
            ResultType = resultType;
            Source = source;

            Source.Attach(this);
        }

        public override void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
        {
            var toAssert = new TupleFactList();
            foreach (var tuple in tuples)
            {
                AssertBinding(context, tuple, toAssert);
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            var toAssert = new TupleFactList();
            var toUpdate = new TupleFactList();
            var toRetract = new TupleFactList();
            foreach (var tuple in tuples)
            {
                var fact = tuple.GetState<Fact>(this);
                if (fact != null)
                {
                    UpdateBinding(context, tuple, fact, toUpdate, toRetract);
                }
                else
                {
                    AssertBinding(context, tuple, toAssert);
                }
            }
            MemoryNode.PropagateRetract(context, toRetract);
            MemoryNode.PropagateUpdate(context, toUpdate);
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            foreach (var tuple in tuples)
            {
                RetractBinding(tuple, toRetract);
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        private void AssertBinding(IExecutionContext context, Tuple tuple, TupleFactList toAssert)
        {
            try
            {
                var value = BindingExpression.Invoke(tuple);
                var fact = new Fact(value, ResultType);
                tuple.SetState(this, fact);
                toAssert.Add(tuple, fact);
            }
            catch (Exception e)
            {
                bool isHandled = false;
                context.EventAggregator.RaiseBindingFailed(context.Session, e, BindingExpression.Expression, tuple, ref isHandled);
                if (!isHandled)
                {
                    throw new RuleExpressionEvaluationException("Failed to evaluate binding expression",
                        BindingExpression.Expression.ToString(), e);
                }
            }
        }

        private void UpdateBinding(IExecutionContext context, Tuple tuple, Fact fact, TupleFactList toUpdate, TupleFactList toRetract)
        {
            try
            {
                var value = BindingExpression.Invoke(tuple);
                fact.RawObject = value;
                toUpdate.Add(tuple, fact);
            }
            catch (Exception e)
            {
                bool isHandled = false;
                context.EventAggregator.RaiseBindingFailed(context.Session, e, BindingExpression.Expression, tuple, ref isHandled);
                if (!isHandled)
                {
                    throw new RuleExpressionEvaluationException("Failed to evaluate binding expression",
                        BindingExpression.Expression.ToString(), e);
                }
                RetractBinding(tuple, toRetract);
            }
        }

        private void RetractBinding(Tuple tuple, TupleFactList toRetract)
        {
            var fact = tuple.RemoveState<Fact>(this);
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
