using BenchmarkDotNet.Running;
using NRules.Benchmark.Expressions;
using NRules.Benchmark.Meta;

namespace NRules.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkOneFactRule>();
            BenchmarkRunner.Run<BenchmarkTwoFactJoinRule>();
            BenchmarkRunner.Run<BenchmarkTwoFactAggregateRule>();
            BenchmarkRunner.Run<BenchmarkMultipleRules>();

            BenchmarkRunner.Run<BenchmarkLhsExpression>();
            BenchmarkRunner.Run<BenchmarkRuleAction>();
        }
    }
}
