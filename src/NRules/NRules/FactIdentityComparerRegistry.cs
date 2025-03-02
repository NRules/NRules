using System;
using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules;

/// <summary>
/// Registry of custom equality comparers for specific fact types used by
/// <see cref="IFactIdentityComparer"/> when managing facts in the rules session.
/// </summary>
public class FactIdentityComparerRegistry
{
    private readonly Dictionary<Type, Entry> _comparers = new();

    /// <summary>
    /// Default equality comparer used to compare fact identity, when inserting, updating, removing
    /// facts within the rules session.
    /// Used by <see cref="IFactIdentityComparer"/> when no custom comparer is registered for the fact type.
    /// </summary>
    /// <remarks>
    /// Built-in implementation uses <see cref="IIdentityProvider"/> if implemented by the fact,
    /// to compare fact identity. Otherwise, it uses the fact's equality for identity comparison.
    /// </remarks>
    public IEqualityComparer<object> DefaultFactIdentityComparer { get; set; } = new DefaultFactIdentityComparer();

    /// <summary>
    /// Register a custom fact identity comparer for a specific fact type.
    /// Used by <see cref="IFactIdentityComparer"/> to compare identity of facts of the specified type.
    /// </summary>
    /// <param name="comparer">Custom fact identity comparer.</param>
    /// <typeparam name="TFact">Type of fact for which identity comparer is registered.</typeparam>
    /// <remarks>
    /// Custom comparers are not polymorphic. They are used only for the exact type they are registered for.
    /// </remarks>
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

    private sealed class SpecificComparerWrapper<TFact>(IEqualityComparer<TFact> comparer)
        : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return comparer.Equals((TFact)x, (TFact)y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return comparer.GetHashCode((TFact)obj);
        }
    }
}