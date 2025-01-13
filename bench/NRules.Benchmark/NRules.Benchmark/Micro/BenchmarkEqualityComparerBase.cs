using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NRules.RuleModel;

namespace NRules.Benchmark.Micro;

[MemoryDiagnoser]
public abstract class BenchmarkEqualityComparerBase
{
    protected readonly IEqualityComparer<object> DefaultEqualityComparer = EqualityComparer<object>.Default;
    protected readonly IEqualityComparer<object> IdentityEqualityComparer;
    protected readonly IEqualityComparer<IdentityFactType> IdentityTypedEqualityComparer;
    protected readonly IEqualityComparer<object> CustomEqualityComparer;
    protected readonly IEqualityComparer<FactType> CustomTypedEqualityComparer;
    
    protected readonly FactType FactType1 = new("value1");
    protected readonly FactType FactType2 = new("value2");    
    protected readonly EquatableFactType EquatableFactType1 = new("value1");
    protected readonly EquatableFactType EquatableFactType2 = new("value2");
    protected readonly IdentityFactType IdentityFactType1 = new("value1");
    protected readonly IdentityFactType IdentityFactType2 = new("value2");
    
    protected BenchmarkEqualityComparerBase()
    {
        var comparerRegistry = new FactIdentityComparerRegistry();
        
        var identityEqualityComparer = new FactIdentityComparer(
            comparerRegistry.DefaultFactIdentityComparer,
            comparerRegistry.GetComparers());
        IdentityEqualityComparer = identityEqualityComparer;
        IdentityTypedEqualityComparer = identityEqualityComparer.GetComparer<IdentityFactType>();
        
        comparerRegistry.RegisterComparer(new FactTypeEqualityComparer());
        var customEqualityComparer = new FactIdentityComparer(
            comparerRegistry.DefaultFactIdentityComparer,
            comparerRegistry.GetComparers());
        CustomEqualityComparer = customEqualityComparer;
        CustomTypedEqualityComparer = customEqualityComparer.GetComparer<FactType>();
    }
    
    public class FactType(string value)
    {
        public string Value { get; } = value;
    }
    
    public class FactTypeEqualityComparer : IEqualityComparer<FactType>
    {
        public bool Equals(FactType? x, FactType? y)
        {
            return x?.Value == y?.Value;
        }

        public int GetHashCode(FactType obj)
        {
            return obj.Value.GetHashCode();
        }
    }
    
    public class EquatableFactType(string value) : IEquatable<EquatableFactType>
    {
        public string Value { get; } = value;

        public bool Equals(EquatableFactType? other)
        {
            return Value == other?.Value;
        }
        
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    
    public class IdentityFactType(string value) : IIdentityProvider
    {
        public string Value { get; } = value;

        public object GetIdentity()
        {
            return Value;
        }
    }
}