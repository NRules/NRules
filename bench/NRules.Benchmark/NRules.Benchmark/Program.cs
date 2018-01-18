using BenchmarkDotNet.Running;

namespace NRules.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<OneFactRule>();
        }
    }
}
