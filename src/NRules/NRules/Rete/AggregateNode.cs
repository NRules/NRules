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

        public override void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(workingMemory);
            Fact aggregateFact = workingMemory.GetFact(aggregate.Result);
            PropagateAggregateAssert(workingMemory, tuple, aggregateFact);
        }

        public override void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(workingMemory);
            Fact aggregateFact = workingMemory.GetFact(aggregate.Result);
            PropagateAggregateUpdate(workingMemory, tuple, aggregateFact);
        }

        public override void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple)
        {
            IAggregate aggregate = GetAggregate(workingMemory);
            Fact aggregateFact = workingMemory.GetFact(aggregate.Result);
            PropagateAggregateRetract(workingMemory, tuple, aggregateFact);
        }

        public override void PropagateAssert(IWorkingMemory workingMemory, Fact fact)
        {
            IAggregate aggregate = GetAggregate(workingMemory);
            var result = aggregate.Add(fact.Object);

            var tuples = LeftSource.GetTuples(workingMemory);
            foreach (var tuple in tuples)
            {
                HandleAggregateResult(workingMemory, result, tuple, aggregate);
            }
        }

        public override void PropagateUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            IAggregate aggregate = GetAggregate(workingMemory);
            var result = aggregate.Modify(fact.Object);

            var tuples = LeftSource.GetTuples(workingMemory);
            foreach (var tuple in tuples)
            {
                HandleAggregateResult(workingMemory, result, tuple, aggregate);
            }
        }

        public override void PropagateRetract(IWorkingMemory workingMemory, Fact fact)
        {
            IAggregate aggregate = GetAggregate(workingMemory);
            var result = aggregate.Remove(fact.Object);

            var tuples = LeftSource.GetTuples(workingMemory);
            foreach (var tuple in tuples)
            {
                HandleAggregateResult(workingMemory, result, tuple, aggregate);
            }
        }

        private void HandleAggregateResult(IWorkingMemory workingMemory, AggregationResults result, Tuple leftTuple, IAggregate aggregate)
        {
            Fact aggregateFact = GetAggregateFact(workingMemory, aggregate);
            switch (result)
            {
                case AggregationResults.Added:
                    PropagateAggregateAssert(workingMemory, leftTuple, aggregateFact);
                    break;
                case AggregationResults.Modified:
                    PropagateAggregateUpdate(workingMemory, leftTuple, aggregateFact);
                    break;
                case AggregationResults.Removed:
                    PropagateAggregateRetract(workingMemory, leftTuple, aggregateFact);
                    break;
            }
        }

        private void PropagateAggregateAssert(IWorkingMemory workingMemory, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                var newTuple = new Tuple(tuple, aggregateFact);
                Sink.PropagateAssert(workingMemory, newTuple);
            }
        }

        private void PropagateAggregateUpdate(IWorkingMemory workingMemory, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                Tuple childTuple = tuple.ChildTuples.FirstOrDefault(t => t.RightFact == aggregateFact);
                if (childTuple != null)
                {
                    Sink.PropagateUpdate(workingMemory, childTuple);
                }
            }
        }

        private void PropagateAggregateRetract(IWorkingMemory workingMemory, Tuple tuple, Fact aggregateFact)
        {
            if (aggregateFact != null)
            {
                Tuple childTuple = tuple.ChildTuples.FirstOrDefault(t => t.RightFact == aggregateFact);
                if (childTuple != null)
                {
                    Sink.PropagateRetract(workingMemory, childTuple);
                }
            }
        }
        
        private IAggregate GetAggregate(IWorkingMemory workingMemory)
        {
            var aggregate = (IAggregate) workingMemory.GetNodeState(this);
            if (aggregate == null)
            {
                aggregate = _aggregateFactory();
                workingMemory.SetNodeState(this, aggregate);
            }
            return aggregate;
        }

        private Fact GetAggregateFact(IWorkingMemory workingMemory, IAggregate aggregate)
        {
            if (aggregate.Result == null) return null;
            Fact fact = workingMemory.GetFact(aggregate.Result);
            if (fact == null)
            {
                fact = new Fact(aggregate.Result);
                workingMemory.SetFact(fact);
            }
            return fact;
        }
    }
}