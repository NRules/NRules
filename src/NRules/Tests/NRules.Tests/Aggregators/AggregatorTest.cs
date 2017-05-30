using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Tests.Aggregators
{
    public abstract class AggregatorTest
    {
        protected IEnumerable<IFact> AsFact<T>(params T[] value)
        {
            return value.Select(x => new Fact(x));
        }

        protected ITuple EmptyTuple()
        {
            return new NullTuple();
        }

        private class NullTuple : ITuple
        {
            public IFact RightFact => null;
            public ITuple LeftTuple => null;
            public IEnumerable<IFact> Facts => new IFact[0];
        }

        private class Fact : IFact
        {
            public Fact(object value)
            {
                Type = value.GetType();
                Value = value;
            }

            public Type Type { get; }
            public object Value { get; }
        }
    }
}