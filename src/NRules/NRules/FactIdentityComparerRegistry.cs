using System;
using System.Collections.Generic;

namespace NRules;

/// <summary>
/// Registry of custom equality comparers for specific fact types.
/// </summary>
public class FactIdentityComparerRegistry
{
    private readonly Dictionary<Type, Entry> _comparers = new();

    /// <summary>
    /// Register a custom fact identity comparer for a specific fact type.
    /// </summary>
    /// <param name="comparer">Custom fact identity comparer.</param>
    /// <typeparam name="TFact">Type of fact for which identity comparer is registered.</typeparam>
    public void RegisterComparer<TFact>(IEqualityComparer<TFact> comparer)
    {
        var wrappedComparer = new SpecificComparerWrapper<TFact>(comparer);
        _comparers[typeof(TFact)] = new Entry(typeof(TFact), comparer, wrappedComparer);
    }
    
    internal IReadOnlyCollection<Entry> GetComparers()
    {
        return _comparers.Values;
    }

    internal class Entry(Type factType, object comparer, IEqualityComparer<object> wrappedComparer)
    {
        public Type FactType { get; } = factType;
        public object Comparer { get; } = comparer;
        public IEqualityComparer<object> WrappedComparer { get; } = wrappedComparer;
    }

    private class SpecificComparerWrapper<TFact> : IEqualityComparer<object>
    {
        private readonly IEqualityComparer<TFact> _comparer;

        public SpecificComparerWrapper(IEqualityComparer<TFact> comparer)
        {
            _comparer = comparer;
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return _comparer.Equals((TFact)x, (TFact)y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return _comparer.GetHashCode((TFact)obj);
        }
    }
}