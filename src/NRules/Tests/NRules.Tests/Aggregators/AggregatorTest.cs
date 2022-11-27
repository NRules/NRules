using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using NRules.Diagnostics;
using NRules.RuleModel;

namespace NRules.Tests.Aggregators;

public abstract class AggregatorTest
{
    protected AggregationContext Context { get; } = new AggregationContext(new FakeExecutionContext(), new Diagnostics.NodeInfo());

    protected Fact[] AsFact<T>(params T[] value)
        where T : notnull
    {
        return value.Select(x => new Fact(x)).ToArray();
    }

    protected ITuple EmptyTuple()
    {
        return new NullTuple();
    }

    private class NullTuple : ITuple
    {
        public IEnumerable<IFact> Facts => Array.Empty<IFact>();
        public int Count => 0;
    }

    public class Fact : IFact
    {
        public Fact(object value)
        {
            Type = value.GetType();
            Value = value;
        }

        public Type Type { get; }
        public object Value { get; set; }
        public IFactSource? Source { get; set; }
    }

    private sealed class FakeExecutionContext : IExecutionContext
    {
        public ISessionInternal Session => throw new NotImplementedException();
        public IWorkingMemory WorkingMemory => throw new NotImplementedException();
        public IAgendaInternal Agenda => throw new NotImplementedException();
        public IEventAggregator EventAggregator => throw new NotImplementedException();
        public IMetricsAggregator MetricsAggregator => throw new NotImplementedException();
        public IIdGenerator IdGenerator => throw new NotImplementedException();
    }
}

public class FactExpression<TFact, TResult> : IAggregateExpression
    where TResult : notnull
{
    private readonly Func<TFact, TResult> _func;

    public FactExpression(Func<TFact, TResult> func)
    {
        _func = func;
        Name = GetType().Name;
    }

    public string Name { get; }

    public object Invoke(AggregationContext context, ITuple tuple, IFact fact)
    {
        var value = fact.Value ?? throw new InvalidOperationException("Fact contains null value");
        return _func((TFact)value);
    }
}