namespace NRules.Utilities;

#if NETSTANDARD2_0
public static class KeyValuePairExtensions
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey key, out TValue value)
    {
        key = source.Key;
        value = source.Value;
    }
}
#endif
