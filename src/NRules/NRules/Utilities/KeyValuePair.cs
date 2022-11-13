namespace NRules.Utilities;
#if NETSTANDARD2_0
public static class KeyValuePair
{
    public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) => new(key, value);
}
#endif
