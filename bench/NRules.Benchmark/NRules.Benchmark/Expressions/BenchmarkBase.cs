using BenchmarkDotNet.Attributes;
using NRules.Diagnostics;
using NRules.Rete;

namespace NRules.Benchmark.Expressions
{
    [MemoryDiagnoser]
    public abstract class BenchmarkBase
    {
        internal IExecutionContext Context;

        protected BenchmarkBase()
        {
            Context = new ExecutionContext(null, null, null, new EventAggregator(), null, null);
        }

        internal static Tuple ToTuple(params object[] values)
        {
            int i = 0;
            var tuple = new Tuple(i);

            foreach (var value in values)
            {
                tuple = new Tuple(++i, tuple, new Fact(value));
            }

            return tuple;
        }
    }
}
