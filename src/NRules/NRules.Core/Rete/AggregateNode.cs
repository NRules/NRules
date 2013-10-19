using System;
using System.Linq;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal class AggregateNode : BetaNode
    {
        private readonly Func<IAggregate> _aggregateFactory;

        public AggregateNode(ITupleSource leftSource, IObjectSource rightSource, Type aggregateType)
            : base(leftSource, rightSource)
        {
            _aggregateFactory = () => (IAggregate) Activator.CreateInstance(aggregateType);
        }

        protected override void PropagateMatchedAssert(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            IAggregate aggregate = GetAggregate(leftTuple);
            var result = aggregate.Add(rightFact.Object);
            HandleAggregateResult(workingMemory, result, leftTuple, aggregate);
        }

        protected override void PropagateMatchedUpdate(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            IAggregate aggregate = GetAggregate(leftTuple);
            var result = aggregate.Modify(rightFact.Object);
            HandleAggregateResult(workingMemory, result, leftTuple, aggregate);
        }

        protected override void PropagateMatchedRetract(IWorkingMemory workingMemory, Tuple leftTuple, Fact rightFact)
        {
            IAggregate aggregate = GetAggregate(leftTuple);
            var result = aggregate.Remove(rightFact.Object);
            HandleAggregateResult(workingMemory, result, leftTuple, aggregate);
        }

        private void HandleAggregateResult(IWorkingMemory workingMemory, AggregationResults result, Tuple leftTuple,
                                           IAggregate aggregate)
        {
            switch (result)
            {
                case AggregationResults.Added:
                    PropagateAggregateAssert(workingMemory, leftTuple, aggregate);
                    break;
                case AggregationResults.Modified:
                    PropagateAggregateUpdate(workingMemory, leftTuple, aggregate);
                    break;
                case AggregationResults.Removed:
                    PropagateAggregateRetract(workingMemory, leftTuple, aggregate);
                    break;
            }
        }

        private void PropagateAggregateAssert(IWorkingMemory workingMemory, Tuple leftTuple, IAggregate aggregate)
        {
            var fact = new Fact(aggregate.Result);
            workingMemory.SetFact(fact);
            var newTuple = CreateTuple(leftTuple, fact);
            MemoryNode.PropagateAssert(workingMemory, newTuple);
        }

        private void PropagateAggregateUpdate(IWorkingMemory workingMemory, Tuple leftTuple, IAggregate aggregate)
        {
            Fact fact = workingMemory.GetFact(aggregate.Result);
            var childTuples = leftTuple.ChildTuples.Where(t => t.RightFact == fact).ToList();
            foreach (var childTuple in childTuples)
            {
                MemoryNode.PropagateUpdate(workingMemory, childTuple);
            }
        }

        private void PropagateAggregateRetract(IWorkingMemory workingMemory, Tuple leftTuple, IAggregate aggregate)
        {
            Fact fact = workingMemory.GetFact(aggregate.Result);
            workingMemory.RemoveFact(fact);
            var childTuples = leftTuple.ChildTuples.Where(t => t.RightFact == fact).ToList();
            foreach (var childTuple in childTuples)
            {
                MemoryNode.PropagateRetract(workingMemory, childTuple);
            }
        }

        private IAggregate GetAggregate(Tuple tuple)
        {
            var aggregate = tuple.GetStateObject<IAggregate>();
            if (aggregate == null)
            {
                aggregate = _aggregateFactory();
                tuple.SetStateObject(aggregate);
            }
            return aggregate;
        }
    }
}