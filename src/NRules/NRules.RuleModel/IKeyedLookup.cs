namespace NRules.RuleModel;

/// <summary>
/// Collection of facts grouped by a key.
/// Exposes all keys present in the lookup as a <see cref="Keys"/> collection.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the lookup.</typeparam>
/// <typeparam name="TElement">The type of the elements in the lookup,</typeparam>
public interface IKeyedLookup<TKey, TElement> : ILookup<TKey, TElement>
{
    /// <summary>
    /// All keys present in the lookup.
    /// To find the number of keys in the lookup use <see cref="ILookup{TKey,TElement}.Count"/>.
    /// </summary>
    IEnumerable<TKey> Keys { get; }
}
