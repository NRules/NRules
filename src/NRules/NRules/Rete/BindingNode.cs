using System.Collections.Generic;

namespace NRules.Rete
{
    internal class BindingNode : IBetaNode, ITupleSink
    {
        public IBindingExpression BindingExpression { get; }
        public ITupleSource Source { get; }
        public IBetaMemoryNode MemoryNode { get; set; }

        public BindingNode(IBindingExpression bindingExpression, ITupleSource source)
        {
            BindingExpression = bindingExpression;
            Source = source;

            Source.Attach(this);
        }

        public void PropagateAssert(IExecutionContext context, IList<Tuple> tuples)
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

        public void PropagateUpdate(IExecutionContext context, IList<Tuple> tuples)
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

        public void PropagateRetract(IExecutionContext context, IList<Tuple> tuples)
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

        public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitBindingNode(context, this);
        }
    }
}
