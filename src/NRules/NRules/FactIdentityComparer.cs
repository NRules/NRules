using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules;

/// <summary>
/// Equality comparer used to compare fact identity, when inserting, updating, removing
/// facts within the rules session.
/// </summary>
/// <remarks>
/// The comparer uses the following rules to compare identity of the facts:
/// <list type="bullet">
/// <item>If both facts are the same reference, they are considered identical.</item>
/// <item>If facts are of different types, they are considered not identical.</item>
/// <item>If custom comparer is registered for the fact type in <see cref="FactIdentityComparerRegistry"/>,
/// it is used to compare the identity of the facts.</item>
/// <item>In all other cases <see cref="FactIdentityComparerRegistry.DefaultFactIdentityComparer"/> is used
/// to compare the identity of the facts.
/// It checks if facts implement <see cref="IIdentityProvider"/> and compares the identity objects in this case.
/// Otherwise, the facts are compared using their equality.</item>
/// </list>
/// </remarks>
public interface IFactIdentityComparer : IEqualityComparer<object>
{
    /// <summary>
    /// Strongly typed fact identity comparer for a specific fact type.
    /// </summary>
    /// <typeparam name="TFact">Type of fact to get an identity comparer for.</typeparam>
    /// <returns>Strongly typed fact identity comparer.</returns>
    IEqualityComparer<TFact> GetComparer<TFact>();
}

internal class FactIdentityComparer : IFactIdentityComparer
{
    private readonly IEqualityComparer<object> _defaultComparer;
    private readonly Dictionary<Type, (object Comparer, IEqualityComparer<object> WrappedComparer)>? _customComparers;
    private readonly Dictionary<Type, object> _typedComparers = new();
    
    public FactIdentityComparer(IEqualityComparer<object> defaultComparer,
        IReadOnlyCollection<FactIdentityComparerRegistry.Entry> customComparers)
    {
        _defaultComparer = defaultComparer;
        if (customComparers.Any())
            _customComparers = customComparers.ToDictionary(x => x.FactType, x => (x.Comparer, x.WrappedComparer));
    }
    
    public IEqualityComparer<TFact> GetComparer<TFact>()
    {
        IEqualityComparer<TFact> typedComparer;

        if (_typedComparers.TryGetValue(typeof(TFact), out var comparer))
        {
            typedComparer = (IEqualityComparer<TFact>)comparer;
        }
        else
        {
            if (_customComparers != null && _customComparers.TryGetValue(typeof(TFact), out var item))
                typedComparer = new SpecificComparerWrapper<TFact>((IEqualityComparer<TFact>)item.Comparer);
            else
                typedComparer = new DefaultComparerWrapper<TFact>(_defaultComparer);
            _typedComparers[typeof(TFact)] = typedComparer;
        }

        return typedComparer;
    }

    bool IEqualityComparer<object>.Equals(object? obj1, object? obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (obj1 is null || obj2 is null)
            return false;
        if (obj1.GetType() != obj2.GetType())
            return false;
        if (_customComparers != null && _customComparers.TryGetValue(obj1.GetType(), out var item))
            return item.WrappedComparer.Equals(obj1, obj2);
        return _defaultComparer.Equals(obj1, obj2);
    }

    int IEqualityComparer<object>.GetHashCode(object? obj)
    {
        if (obj == null)
            return 0;
        if (_customComparers != null && _customComparers.TryGetValue(obj.GetType(), out var item))
            return item.WrappedComparer.GetHashCode(obj);
        return _defaultComparer.GetHashCode(obj);
    }

    private class DefaultComparerWrapper<TFact> : IEqualityComparer<TFact>
    {
        private readonly IEqualityComparer<object> _comparer;

        public DefaultComparerWrapper(IEqualityComparer<object> comparer)
        {
            _comparer = comparer;
        }

        public bool Equals(TFact obj1, TFact obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (obj1 is null || obj2 is null)
                return false;
            if (obj1.GetType() != obj2.GetType())
                return false;
            return _comparer.Equals(obj1, obj2);
        }

        public int GetHashCode(TFact obj)
        {
            if (obj == null)
                return 0;
            return _comparer.GetHashCode(obj);
        }
    }
    
    private class SpecificComparerWrapper<TFact> : IEqualityComparer<TFact>
    {
        private readonly IEqualityComparer<TFact> _comparer;

        public SpecificComparerWrapper(IEqualityComparer<TFact> comparer)
        {
            _comparer = comparer;
        }

        public bool Equals(TFact obj1, TFact obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (obj1 is null || obj2 is null)
                return false;
            if (obj1.GetType() != obj2.GetType())
                return false;
            return _comparer.Equals(obj1, obj2);
        }

        public int GetHashCode(TFact obj)
        {
            if (obj == null)
                return 0;
            return _comparer.GetHashCode(obj);
        }
    }
}

internal class DefaultFactIdentityComparer : IEqualityComparer<object>
{
    bool IEqualityComparer<object>.Equals(object? obj1, object? obj2)
    {
        if (obj1 is IIdentityProvider provider1 && obj2 is IIdentityProvider provider2)
            return Equals(provider1.GetIdentity(), provider2.GetIdentity());

        return Equals(obj1, obj2);
    }

    int IEqualityComparer<object>.GetHashCode(object obj)
    {
        if (obj is IIdentityProvider provider)
            return provider.GetIdentity().GetHashCode();

        return obj.GetHashCode();
    }
}