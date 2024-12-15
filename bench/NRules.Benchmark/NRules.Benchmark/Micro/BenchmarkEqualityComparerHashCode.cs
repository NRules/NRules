using BenchmarkDotNet.Attributes;

namespace NRules.Benchmark.Micro;

[BenchmarkCategory("Micro", "Equality")]
public class BenchmarkEqualityComparerHashCode : BenchmarkEqualityComparerBase
{
    [Benchmark(Baseline = true)]
    public int DotnetDefault()
    {
        return DefaultEqualityComparer.GetHashCode(FactType1);
    }
    
    [Benchmark]
    public int DotnetDefaultEquatable()
    {
        return DefaultEqualityComparer.GetHashCode(EquatableFactType1);
    }
    
    [Benchmark]
    public int FactDefault()
    {
        return IdentityEqualityComparer.GetHashCode(FactType1);
    }
    
    [Benchmark]
    public int FactDefaultEquatable()
    {
        return IdentityEqualityComparer.GetHashCode(EquatableFactType1);
    }
    
    [Benchmark]
    public int FactDefaultIdentity()
    {
        return IdentityEqualityComparer.GetHashCode(IdentityFactType1);
    }
    
    [Benchmark]
    public int FactDefaultTypedIdentity()
    {
        return IdentityTypedEqualityComparer.GetHashCode(IdentityFactType1);
    }
    
    [Benchmark]
    public int FactCustom()
    {
        return CustomEqualityComparer.GetHashCode(FactType1);
    }
    
    [Benchmark]
    public int FactCustomEquatable()
    {
        return CustomEqualityComparer.GetHashCode(EquatableFactType1);
    }
    
    [Benchmark]
    public int FactCustomIdentity()
    {
        return CustomEqualityComparer.GetHashCode(IdentityFactType1);
    }
    
    [Benchmark]
    public int FactCustomTyped()
    {
        return CustomTypedEqualityComparer.GetHashCode(FactType1);
    }
}