using System.Collections.Generic;

namespace NRules.Rete
{
    internal class BindingNode : BetaNode
    {
        public IBindingExpression BindingExpression { get; }
        public ITupleSource Source { get; }

        public BindingNode(IBindingExpression bindingExpression, ITupleSource source)
        {
            BindingExpression = bindingExpression;
            Source = source;

            Source.Attach(this);
        }

        public override void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
        {
            var toAssert = new TupleFactList();
            foreach (var tuple in tuples)
            {
                var value = BindingExpression.Invoke(context, tuple);
                var fact = new Fact(value);
                tuple.SetState(this, fact);
                context.WorkingMemory.AddInternalFact(this, fact);
                toAssert.Add(tuple, fact);
            }
            if (toAssert.Count > 0)
            {
                MemoryNode.PropagateAssert(context, toAssert);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
        {
            var toUpdate = new TupleFactList();
            foreach (var tuple in tuples)
            {
                var fact = tuple.GetState<Fact>(this);
                var oldValue = fact.RawObject;
                var newValue = BindingExpression.Invoke(context, tuple);

                if (!Equals(oldValue, newValue))
                {
                    context.WorkingMemory.RemoveInternalFact(this, fact);
                    fact.RawObject = newValue;
                    context.WorkingMemory.AddInternalFact(this, fact);
                }
                if (!ReferenceEquals(oldValue, newValue))
                {
                    fact.RawObject = newValue;
                    context.WorkingMemory.UpdateInternalFact(this, fact);
                }

                toUpdate.Add(tuple, fact);
            }
            if (toUpdate.Count > 0)
            {
                MemoryNode.PropagateUpdate(context, toUpdate);
            }
        }

        public override void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
        {
            var toRetract = new TupleFactList();
            foreach (var tuple in tuples)
            {
                var fact = tuple.GetState<Fact>(this);
                toRetract.Add(tuple, fact);
            }
            if (toRetract.Count > 0)
            {
                MemoryNode.PropagateRetract(context, toRetract);
                var enumerator = toRetract.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    context.WorkingMemory.RemoveInternalFact(this, enumerator.CurrentFact);
                }
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBindingNode(context, this);
        }
    }
}
