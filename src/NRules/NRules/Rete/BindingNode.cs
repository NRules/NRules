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
                var value = BindingExpression.Invoke(context, tuple);
                var fact = new Fact(value, ResultType);
                tuple.SetState(this, fact);
                toAssert.Add(tuple, fact);
            }
            MemoryNode.PropagateAssert(context, toAssert);
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            var toUpdate = new TupleFactList();
            foreach (var tuple in tuples)
            {
                var fact = tuple.GetState<Fact>(this);
                var oldValue = fact.RawObject;
                var newValue = BindingExpression.Invoke(context, tuple);

                if (!ReferenceEquals(oldValue, newValue))
                {
                    fact.RawObject = newValue;
                }
                toUpdate.Add(tuple, fact);
            }
            MemoryNode.PropagateUpdate(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            foreach (var tuple in tuples)
            {
                var fact = tuple.GetState<Fact>(this);
                toRetract.Add(tuple, fact);
            }
            MemoryNode.PropagateRetract(context, toRetract);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBindingNode(context, this);
        }
    }
}
