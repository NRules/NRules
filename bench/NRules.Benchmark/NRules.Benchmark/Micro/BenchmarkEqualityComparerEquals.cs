using BenchmarkDotNet.Attributes;

namespace NRules.Benchmark.Micro;

[BenchmarkCategory("Micro", "Equality")]
public class BenchmarkEqualityComparerEquals : BenchmarkEqualityComparerBase
{
    [Benchmark(Baseline = true)]
    public bool DotnetDefault()
    {
        return DefaultEqualityComparer.Equals(FactType1, FactType2);
    }
    
    [Benchmark]
    public bool DotnetDefaultEquatable()
    {
        return DefaultEqualityComparer.Equals(EquatableFactType1, EquatableFactType2);
    }
    
    [Benchmark]
    public bool FactDefault()
    {
        return IdentityEqualityComparer.Equals(FactType1, FactType2);
    }
    
    [Benchmark]
    public bool FactDefaultEquatable()
    {
        return IdentityEqualityComparer.Equals(EquatableFactType1, EquatableFactType2);
    }
    
    [Benchmark]
    public bool FactDefaultIdentity()
    {
        return IdentityEqualityComparer.Equals(IdentityFactType1, IdentityFactType2);
    }
    
    [Benchmark]
    public bool FactDefaultTypedIdentity()
    {
        return IdentityTypedEqualityComparer.Equals(IdentityFactType1, IdentityFactType2);
    }
    
    [Benchmark]
    public bool FactCustom()
    {
        return CustomEqualityComparer.Equals(FactType1, FactType2);
    }
    
    [Benchmark]
    public bool FactCustomEquatable()
    {
        return CustomEqualityComparer.Equals(EquatableFactType1, EquatableFactType2);
    }
    
    [Benchmark]
    public bool FactCustomIdentity()
    {
        return CustomEqualityComparer.Equals(IdentityFactType1, IdentityFactType2);
    }
    
    [Benchmark]
    public bool FactCustomTyped()
    {
        return CustomTypedEqualityComparer.Equals(FactType1, FactType2);
    }
}