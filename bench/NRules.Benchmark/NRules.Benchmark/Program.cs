using BenchmarkDotNet.Running;
using NRules.Benchmark.Meta;

namespace NRules.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<OneFactRule>();
            BenchmarkRunner.Run<TwoFactJoinRule>();
            BenchmarkRunner.Run<TwoFactAggregateRule>();
            BenchmarkRunner.Run<MultipleRules>();
        }
    }
}
