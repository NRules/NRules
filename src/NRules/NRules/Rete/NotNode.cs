using System.Linq;

namespace NRules.Rete
{
    internal class NotNode : BetaNode
    {
        public NotNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource)
        {
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            if (!RightSource.GetFacts(context).Any())
            {
                Sink.PropagateAssert(context, tuple);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            if (!RightSource.GetFacts(context).Any())
            {
                Sink.PropagateUpdate(context, tuple);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            if (!RightSource.GetFacts(context).Any())
            {
                Sink.PropagateRetract(context, tuple);
            }
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            if (RightSource.GetFacts(context).Count() == 1)
            {
                var tuples = LeftSource.GetTuples(context);
                foreach (var tuple in tuples)
                {
                    Sink.PropagateRetract(context, tuple);
                }
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            //Do nothing
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            if (!RightSource.GetFacts(context).Any())
            {
                var tuples = LeftSource.GetTuples(context);
                foreach (var tuple in tuples)
                {
                    Sink.PropagateAssert(context, tuple);
                }
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitNotNode(context, this);
        }
    }
}