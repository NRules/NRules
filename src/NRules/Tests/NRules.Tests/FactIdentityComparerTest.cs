using System;
using System.Collections.Generic;
using NRules.RuleModel;
using Xunit;

namespace NRules.Tests;

public class FactIdentityComparerTest
{
    [Fact]
    public void Equals_DefaultComparer_Nulls_True()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(null, null);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_Nulls_True()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactEquatable>();

        // Act
        var result = target.Equals(null, null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DefaultComparer_NullAndObject_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(null, new FactEquatable("1"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_DefaultComparer_ObjectAndNull_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactEquatable("1"), null);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_NullAndObject_False()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactEquatable>();

        // Act
        var result = target.Equals(null, new FactEquatable("1"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_DefaultComparer_DifferentTypesInOneOrder_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithIdentity("1"), new FactEquatable("1"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_DefaultComparer_DifferentTypesInAnotherOrder_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactEquatable("1"), new FactWithIdentity("1"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_DifferentTypesInOneOrder_False()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactWithIdentity>();

        // Act
        var result = target.Equals(new FactWithIdentity("1"), new FactWithIdentityEquatable("1", "1"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_StronglyTypedDefaultComparer_DifferentTypesInAnotherOrder_False()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactWithIdentity>();

        // Act
        var result = target.Equals(new FactWithIdentityEquatable("1", "1"), new FactWithIdentity("1"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_DefaultComparer_SameObject_True()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var fact = new FactEquatable("1");
        var result = target.Equals(fact, fact);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_SameObject_True()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactEquatable>();

        // Act
        var fact = new FactEquatable("1");
        var result = target.Equals(fact, fact);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DefaultComparer_EqualObjectsWithEquatable_True()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactEquatable("1"), new FactEquatable("1"));

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_EqualObjectsWithEquatable_True()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactEquatable>();

        // Act
        var result = target.Equals(new FactEquatable("1"), new FactEquatable("1"));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DefaultComparer_NonEqualObjectsWithEquatable_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactEquatable("1"), new FactEquatable("2"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_NonEqualObjectsWithEquatable_False()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactEquatable>();

        // Act
        var result = target.Equals(new FactEquatable("1"), new FactEquatable("2"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_DefaultComparer_EqualObjectsWithIdentity_True()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithIdentity("1"), new FactWithIdentity("1"));

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedDefaultComparer_EqualObjectsWithIdentity_True()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactWithIdentity>();

        // Act
        var result = target.Equals(new FactWithIdentity("1"), new FactWithIdentity("1"));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DefaultComparer_NonEqualObjectsWithIdentity_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithIdentity("1"), new FactWithIdentity("2"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_StronglyTypedDefaultComparer_NonEqualObjectsWithIdentity_False()
    {
        // Arrange
        var target = CreateTarget().GetComparer<FactWithIdentity>();

        // Act
        var result = target.Equals(new FactWithIdentity("1"), new FactWithIdentity("2"));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_DefaultComparer_EqualObjectsWithSameIdentityAndDifferentEquality_True()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithIdentityEquatable("1", "20"), new FactWithIdentityEquatable("1", "21"));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DefaultComparer_NonEqualObjectsWithDifferentIdentityAndSameEquality_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithIdentityEquatable("1", "20"), new FactWithIdentityEquatable("2", "20"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_DefaultComparer_CustomWithSameIdentityDifferentEquality_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithCustomIdentity(1, "20"), new FactWithCustomIdentity(1, "21"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_CustomDefaultComparer_CustomWithSameIdentityDifferentEquality_True()
    {
        // Arrange
        var target = CreateTarget(x => x.DefaultFactIdentityComparer = new CustomFactIdentityComparer());

        // Act
        var result = target.Equals(new FactWithCustomIdentity(1, "20"), new FactWithCustomIdentity(1, "21"));

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_CustomDefaultComparer_CustomWithDifferentIdentitySameEquality_False()
    {
        // Arrange
        var target = CreateTarget(x => x.DefaultFactIdentityComparer = new CustomFactIdentityComparer());

        // Act
        var result = target.Equals(new FactWithCustomIdentity(1, "20"), new FactWithCustomIdentity(2, "20"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomDefaultComparer_CustomWithSameIdentityDifferentEquality_True()
    {
        // Arrange
        var target = CreateTarget(x => x.DefaultFactIdentityComparer = new CustomFactIdentityComparer())
            .GetComparer<FactWithCustomIdentity>();

        // Act
        var result = target.Equals(new FactWithCustomIdentity(1, "20"), new FactWithCustomIdentity(1, "21"));

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomDefaultComparer_CustomWithDifferentIdentitySameEquality_False()
    {
        // Arrange
        var target = CreateTarget(x => x.DefaultFactIdentityComparer = new CustomFactIdentityComparer())
            .GetComparer<FactWithCustomIdentity>();

        // Act
        var result = target.Equals(new FactWithCustomIdentity(1, "20"), new FactWithCustomIdentity(2, "20"));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_DefaultComparer_FactsWithSameOpaqueIdentity_False()
    {
        // Arrange
        var target = CreateTarget();

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), new FactWithOpaqueIdentity(1));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_DefaultComparerPlusCustomFactComparer_FactsWithSameOpaqueIdentity_True()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()));

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), new FactWithOpaqueIdentity(1));

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_DefaultComparerPlusCustomFactComparer_FactsWithDifferentOpaqueIdentity_False()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()));

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), new FactWithOpaqueIdentity(2));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_Nulls_True()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(null, null);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_NullAndObject_False()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(null, new FactWithOpaqueIdentity(1));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_ObjectAndNull_False()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), null);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_DifferentTypesInOneOrder_False()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), new FactChildWithOpaqueIdentity(1));

        // Assert
        Assert.False(result);
    }
        
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_DifferentTypesInAnotherOrder_False()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(new FactChildWithOpaqueIdentity(1), new FactWithOpaqueIdentity(1));

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_FactsWithSameOpaqueIdentity_True()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), new FactWithOpaqueIdentity(1));

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_StronglyTypedCustomFactComparer_FactsWithDifferentOpaqueIdentity_False()
    {
        // Arrange
        var target = CreateTarget(x => x.RegisterComparer(new FactWithOpaqueIdentityComparer()))
            .GetComparer<FactWithOpaqueIdentity>();

        // Act
        var result = target.Equals(new FactWithOpaqueIdentity(1), new FactWithOpaqueIdentity(2));

        // Assert
        Assert.False(result);
    }

    private static IFactIdentityComparer CreateTarget(Action<FactIdentityComparerRegistry>? action = default)
    {
        var registry = new FactIdentityComparerRegistry();
        action?.Invoke(registry);
        var target = new FactIdentityComparer(
            registry.DefaultFactIdentityComparer,
            registry.GetComparers());
        return target;
    }

    public class FactWithIdentity(string id) : IIdentityProvider
    {
        public string Id { get; } = id;
        public object GetIdentity() => Id;
    }

    public class FactEquatable(string id) : IEquatable<FactEquatable>
    {
        public string Id { get; } = id;

        public bool Equals(FactEquatable? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FactEquatable)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class FactWithIdentityEquatable(string id, string value)
        : FactWithIdentity(id), IEquatable<FactWithIdentityEquatable>
    {
        public string Value { get; } = value;

        public bool Equals(FactWithIdentityEquatable? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FactWithIdentityEquatable)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    
    public interface IHaveCustomIdentity
    {
        int GetIdentity();
    }
    
    public class FactWithCustomIdentity(int id, string value) : IHaveCustomIdentity, IEquatable<FactWithCustomIdentity>
    {
        public int Id { get; } = id;
        public string Value { get; } = value;

        public int GetIdentity() => Id;

        public bool Equals(FactWithCustomIdentity? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FactWithCustomIdentity)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    
    public class CustomFactIdentityComparer : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object? obj1, object? obj2)
        {
            if (obj1 is IHaveCustomIdentity provider1 && obj2 is IHaveCustomIdentity provider2)
                return provider1.GetIdentity() == provider2.GetIdentity();

            return Equals(obj1, obj2);
        }

        int IEqualityComparer<object>.GetHashCode(object? obj)
        {
            if (obj is IHaveCustomIdentity provider)
                return provider.GetIdentity().GetHashCode();
            
            return obj?.GetHashCode() ?? 0;
        }
    }
    
    public class FactWithOpaqueIdentity(int id)
    {
        public int Id { get; } = id;
    }
    
    public class FactChildWithOpaqueIdentity(int id) : FactWithOpaqueIdentity(id)
    {
    }

    public class FactWithOpaqueIdentityComparer : IEqualityComparer<FactWithOpaqueIdentity>
    {
        public bool Equals(FactWithOpaqueIdentity? obj1, FactWithOpaqueIdentity? obj2)
        {
            return obj1!.Id == obj2!.Id;
        }

        public int GetHashCode(FactWithOpaqueIdentity obj)
        {
            return obj.Id;
        }
    }
}