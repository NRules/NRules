using System;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly Func<IAggregate> _aggregateFactory;

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, Type aggregateType)
            : base(leftSource, rightSource)
        {
            _aggregateFactory = () => (IAggregate) Activator.CreateInstance(aggregateType);
        }

        public override void PropagateAssert(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(context);
            Fact aggregateFact = context.WorkingMemory.GetFact(aggregate.Result);
            PropagateAggregateAssert(context, tuple, aggregateFact);
        }

        public override void PropagateUpdate(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(context);
            Fact aggregateFact = context.WorkingMemory.GetFact(aggregate.Result);
            PropagateAggregateUpdate(context, tuple, aggregateFact);
        }

        public override void PropagateRetract(IExecutionContext context, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(context);
            Fact aggregateFact = context.WorkingMemory.GetFact(aggregate.Result);
            PropagateAggregateRetract(context, tuple, aggregateFact);
        }

        public override void PropagateAssert(IExecutionContext context, Fact fact)
        {
            IAggregate aggregate = GetAggregate(context);
            var result = aggregate.Add(fact.Object);

            var tuples = LeftSource.GetTuples(context);
            foreach (var tuple in tuples)
            {
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        public override void PropagateUpdate(IExecutionContext context, Fact fact)
        {
            IAggregate aggregate = GetAggregate(context);
            var result = aggregate.Modify(fact.Object);

            var tuples = LeftSource.GetTuples(context);
            foreach (var tuple in tuples)
            {
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        public override void PropagateRetract(IExecutionContext context, Fact fact)
        {
            IAggregate aggregate = GetAggregate(context);
            var result = aggregate.Remove(fact.Object);

            var tuples = LeftSource.GetTuples(context);
            foreach (var tuple in tuples)
            {
                HandleAggregateResult(context, result, tuple, aggregate);
            }
        }

        private void HandleAggregateResult(IExecutionContext context, AggregationResults result, Tuple leftTuple, IAggregate aggregate)
        {
            Fact aggregateFact = GetAggregateFact(context, aggregate);
            switch (result)
            {
                case AggregationResults.Added:
                    PropagateAggregateAssert(context, leftTuple, aggregateFact);
                    break;
                case AggregationResults.Modified:
                    PropagateAggregateUpdate(context, leftTuple, aggregateFact);
                    break;
                case AggregationResults.Removed:
                    PropagateAggregateRetract(context, leftTuple, aggregateFact);
                    break;
            }
        }

        private void PropagateAggregateAssert(IExecutionContext context, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                var newTuple = new Tuple(tuple, aggregateFact);
                Sink.PropagateAssert(context, newTuple);
            }
        }

        private void PropagateAggregateUpdate(IExecutionContext context, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                Tuple childTuple = tuple.ChildTuples.FirstOrDefault(t => t.RightFact == aggregateFact);
                if (childTuple != null)
                {
                    Sink.PropagateUpdate(context, childTuple);
                }
            }
        }

        private void PropagateAggregateRetract(IExecutionContext context, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                Tuple childTuple = tuple.ChildTuples.FirstOrDefault(t => t.RightFact == aggregateFact);
                if (childTuple != null)
                {
                    Sink.PropagateRetract(context, childTuple);
                }
            }
        }
        
        private IAggregate GetAggregate(IExecutionContext context)
        {
            var aggregate = (IAggregate) context.WorkingMemory.GetNodeState(this);
            if (aggregate == null)
            {
                aggregate = _aggregateFactory();
                context.WorkingMemory.SetNodeState(this, aggregate);
            }
            return aggregate;
        }

        private Fact GetAggregateFact(IExecutionContext context, IAggregate aggregate)
        {
            if (aggregate.Result == null) return null;
            Fact fact = context.WorkingMemory.GetFact(aggregate.Result);
            if (fact == null)
            {
                fact = new Fact(aggregate.Result);
                context.WorkingMemory.SetFact(fact);
            }
            return fact;
        }
    }
}