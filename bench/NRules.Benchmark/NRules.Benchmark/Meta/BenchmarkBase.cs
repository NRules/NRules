using BenchmarkDotNet.Attributes;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Benchmark.Meta
{
    [MemoryDiagnoser]
    [ShortRunJob, WarmupCount(1)]
    public abstract class BenchmarkBase
    {
        protected ISessionFactory Factory { get; set; }

        protected void SetupRule<T>() where T : Rule
        {
            var repository = new RuleRepository();
            repository.Load(x => x.NestedTypes().PrivateTypes().From(xx => xx.Type(typeof(T))));

            Factory = repository.Compile();
        }
    }
}