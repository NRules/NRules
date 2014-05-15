using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NRules.Rete
{
    internal abstract class BetaNode : ITupleSink, IObjectSink
    {
        public ITupleSource LeftSource { get; private set; }
        public IObjectSource RightSource { get; private set; }
        public ITupleSink Sink { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<IBetaCondition> Conditions { get; private set; }

        protected BetaNode(ITupleSource leftSource, IObjectSource rightSource)
        {
            LeftSource = leftSource;
            RightSource = rightSource;

            LeftSource.Attach(this);
            RightSource.Attach(this);

            Conditions = new List<IBetaCondition>();
        }

        public void Attach(ITupleSink sink)
        {
            Sink = sink;
        }

        public abstract void PropagateAssert(IExecutionContext context, Tuple tuple);
        public abstract void PropagateUpdate(IExecutionContext context, Tuple tuple);
        public abstract void PropagateRetract(IExecutionContext context, Tuple tuple);
        public abstract void PropagateAssert(IExecutionContext context, Fact fact);
        public abstract void PropagateUpdate(IExecutionContext context, Fact fact);
        public abstract void PropagateRetract(IExecutionContext context, Fact fact);

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);

        protected IEnumerable<Fact> MatchingFacts(IExecutionContext context, Tuple tuple)
        {
            return RightSource.GetFacts(context).Where(fact => MatchesConditions(tuple, fact));
        }

        protected IEnumerable<Tuple> MatchingTuples(IExecutionContext context, Fact fact)
        {
            return LeftSource.GetTuples(context).Where(tuple => MatchesConditions(tuple, fact));
        }

        protected bool MatchesConditions(Tuple left, Fact right)
        {
            return Conditions.All(joinCondition => joinCondition.IsSatisfiedBy(left, right));
        }
    }
}