using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using NRules.RuleModel;

namespace NRules.Tests.Aggregators
{
    public abstract class AggregatorTest
    {
        protected Fact[] AsFact<T>(params T[] value)
        {
            return value.Select(x => new Fact(x)).ToArray();
        }

        protected ITuple EmptyTuple()
        {
            return new NullTuple();
        }

        private class NullTuple : ITuple
        {
            public IEnumerable<IFact> Facts => new IFact[0];
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
            public IFactSource Source { get; set; }
        }
    }

    public class FactExpression<TFact, TResult> : IAggregateExpression
    {
        private readonly Func<TFact, TResult> _func;

        public FactExpression(Func<TFact, TResult> func)
        {
            _func = func;
        }

        public string Name { get; }

        public object Invoke(AggregationContext context, ITuple tuple, IFact fact)
        {
            return _func((TFact)fact.Value);
        }
    }
}