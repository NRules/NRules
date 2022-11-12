using NRules.Aggregators;
using NRules.Diagnostics;
using NRules.RuleModel;

namespace NRules.Rete;

internal class AggregateNode : BinaryBetaNode
{
    private readonly IAggregatorFactory _aggregatorFactory;
    private readonly bool _isSubnetJoin;

    public string Name { get; }
    public List<Declaration> Declarations { get; }
    public ExpressionCollection Expressions { get; }

    public AggregateNode(
        int id,
        Type outputType,
        ITupleSource leftSource,
        IObjectSource rightSource,
        string name,
        List<Declaration> declarations,
        ExpressionCollection expressions,
        IAggregatorFactory aggregatorFactory,
        bool isSubnetJoin)
        : base(id, outputType, leftSource, rightSource, isSubnetJoin)
    {
        Name = name;
        Declarations = declarations;
        Expressions = expressions;
        _aggregatorFactory = aggregatorFactory;
        _isSubnetJoin = isSubnetJoin;
    }

    public override void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        var aggregationContext = new AggregationContext(context, NodeInfo);
        var aggregation = new Aggregation();

        using (var counter = PerfCounter.Assert(context, this))
        {
            var joinedSets = JoinedSets(context, tuples);
            foreach (var set in joinedSets)
            {
                var aggregator = CreateFactAggregator(context, set.Tuple);
                AddToAggregate(aggregationContext, aggregator, aggregation, set.Tuple, set.Facts);
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(aggregation.Count);
        }

        PropagateAggregation(context, aggregation);
    }

    public override void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        var aggregationContext = new AggregationContext(context, NodeInfo);
        var aggregation = new Aggregation();
        using (var counter = PerfCounter.Update(context, this))
        {
            var joinedSets = JoinedSets(context, tuples);
            foreach (var set in joinedSets)
            {
                var aggregator = GetFactAggregator(context, set.Tuple);
                if (aggregator == null)
                {
                    var matchingFacts = set.Facts;
                    aggregator = CreateFactAggregator(context, set.Tuple);
                    AddToAggregate(aggregationContext, aggregator, aggregation, set.Tuple, matchingFacts);
                }
                else
                {
                    if (_isSubnetJoin && set.Facts.Count > 0)
                    {
                        //Update already propagated from the right
                        continue;
                    }

                    UpdateInAggregate(aggregationContext, aggregator, aggregation, set.Tuple, set.Facts);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(aggregation.Count);
        }

        PropagateAggregation(context, aggregation);
    }

    public override void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        var aggregation = new Aggregation();
        using (var counter = PerfCounter.Retract(context, this))
        {
            foreach (var tuple in tuples)
            {
                var aggregator = RemoveFactAggregator(context, tuple);
                if (aggregator != null)
                {
                    aggregation.Remove(tuple, aggregator.AggregateFacts);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(aggregation.Count);
        }

        PropagateAggregation(context, aggregation);
    }

    public override void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var aggregationContext = new AggregationContext(context, NodeInfo);
        var aggregation = new Aggregation();
        using (var counter = PerfCounter.Assert(context, this))
        {
            var joinedSets = JoinedSets(context, facts).Where(set => set.Facts.Count > 0);
            foreach (var set in joinedSets)
            {
                var aggregator = GetFactAggregator(context, set.Tuple);
                if (aggregator == null)
                {
                    aggregator = CreateFactAggregator(context, set.Tuple);

                    var originalSet = JoinedSet(context, set.Tuple);
                    AddToAggregate(aggregationContext, aggregator, aggregation, originalSet.Tuple, originalSet.Facts);
                }

                AddToAggregate(aggregationContext, aggregator, aggregation, set.Tuple, set.Facts);
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(aggregation.Count);
        }

        PropagateAggregation(context, aggregation);
    }

    public override void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var aggregationContext = new AggregationContext(context, NodeInfo);
        var aggregation = new Aggregation();
        using (var counter = PerfCounter.Update(context, this))
        {
            var joinedSets = JoinedSets(context, facts).Where(set => set.Facts.Count > 0);
            foreach (var set in joinedSets)
            {
                var aggregator = GetFactAggregator(context, set.Tuple);
                if (aggregator == null)
                {
                    var fullSet = JoinedSet(context, set.Tuple);
                    aggregator = CreateFactAggregator(context, fullSet.Tuple);
                    AddToAggregate(aggregationContext, aggregator, aggregation, fullSet.Tuple, fullSet.Facts);
                }
                else
                {
                    UpdateInAggregate(aggregationContext, aggregator, aggregation, set.Tuple, set.Facts);
                }
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(aggregation.Count);
        }

        PropagateAggregation(context, aggregation);
    }

    public override void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var aggregationContext = new AggregationContext(context, NodeInfo);
        var aggregation = new Aggregation();
        using (var counter = PerfCounter.Retract(context, this))
        {
            var joinedSets = JoinedSets(context, facts).Where(set => set.Facts.Count > 0);
            foreach (var set in joinedSets)
            {
                var aggregator = GetFactAggregator(context, set.Tuple);
                if (aggregator != null)
                {
                    RetractFromAggregate(aggregationContext, aggregator, aggregation, set.Tuple, set.Facts);
                }
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(aggregation.Count);
        }

        PropagateAggregation(context, aggregation);
    }

    public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitAggregateNode(context, this);
    }

    private void AddToAggregate(AggregationContext context, IFactAggregator aggregator, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
    {
        try
        {
            aggregator.Add(context, aggregation, tuple, facts);
        }
        catch (ExpressionEvaluationException e)
        {
            if (!e.IsHandled)
            {
                throw new RuleLhsExpressionEvaluationException("Failed to evaluate aggregate expression", e.Expression.ToString(), e.InnerException);
            }
            ResetAggregator(context.ExecutionContext, aggregation, tuple, aggregator);
        }
    }

    private void UpdateInAggregate(AggregationContext context, IFactAggregator aggregator, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
    {
        try
        {
            aggregator.Modify(context, aggregation, tuple, facts);
        }
        catch (ExpressionEvaluationException e)
        {
            if (!e.IsHandled)
            {
                throw new RuleLhsExpressionEvaluationException("Failed to evaluate aggregate expression", e.Expression.ToString(), e.InnerException);
            }
            ResetAggregator(context.ExecutionContext, aggregation, tuple, aggregator);
        }
    }

    private void RetractFromAggregate(AggregationContext context, IFactAggregator aggregator, Aggregation aggregation, Tuple tuple, IEnumerable<Fact> facts)
    {
        try
        {
            aggregator.Remove(context, aggregation, tuple, facts);
        }
        catch (ExpressionEvaluationException e)
        {
            if (!e.IsHandled)
            {
                throw new RuleLhsExpressionEvaluationException("Failed to evaluate aggregate expression", e.Expression.ToString(), e.InnerException);
            }
            ResetAggregator(context.ExecutionContext, aggregation, tuple, aggregator);
        }
    }

    private void ResetAggregator(IExecutionContext context, Aggregation aggregation, Tuple tuple, IFactAggregator aggregator)
    {
        context.WorkingMemory.RemoveState<IFactAggregator>(this, tuple);
        aggregation.Remove(tuple, aggregator.AggregateFacts);
    }

    private void PropagateAggregation(IExecutionContext context, Aggregation aggregation)
    {
        if (MemoryNode is null)
        {
            throw new InvalidOperationException($"{nameof(MemoryNode)} is null");
        }

        foreach (var aggregateList in aggregation.AggregateLists)
        {
            if (aggregateList.Count == 0)
                continue;

            switch (aggregateList.Action)
            {
                case AggregationAction.Added:
                    MemoryNode.PropagateAssert(context, aggregateList);
                    break;
                case AggregationAction.Modified:
                    MemoryNode.PropagateUpdate(context, aggregateList);
                    break;
                case AggregationAction.Removed:
                    MemoryNode.PropagateRetract(context, aggregateList);
                    break;
            }
        }
    }

    private IFactAggregator CreateFactAggregator(IExecutionContext context, Tuple tuple)
    {
        var aggregator = _aggregatorFactory.Create();
        var factAggregator = new FactAggregator(aggregator);
        context.WorkingMemory.SetState(this, tuple, factAggregator);
        return factAggregator;
    }

    private IFactAggregator? GetFactAggregator(IExecutionContext context, Tuple tuple)
    {
        return context.WorkingMemory.GetState<IFactAggregator>(this, tuple);
    }

    private IFactAggregator? RemoveFactAggregator(IExecutionContext context, Tuple tuple)
    {
        return context.WorkingMemory.RemoveState<IFactAggregator>(this, tuple);
    }
}