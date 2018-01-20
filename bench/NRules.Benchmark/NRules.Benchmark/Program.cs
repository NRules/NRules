using BenchmarkDotNet.Running;

namespace NRules.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkOneFactRule>();
            BenchmarkRunner.Run<BenchmarkTwoFactJoinRule>();
            BenchmarkRunner.Run<BenchmarkTwoFactAggregateRule>();
        }
    }
}
