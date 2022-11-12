using System.Collections.Concurrent;

namespace NRules.Json;

internal class TypeCache
{
    private readonly ConcurrentDictionary<string, Type> _nameToTypeCache = new();
    private readonly ConcurrentDictionary<Type, string?> _typeToNameCache = new();

    public string? GetTypeName(Type type, Func<Type, string?> fallback)
    {
        return _typeToNameCache.GetOrAdd(type, fallback);
    }

    public Type GetTypeFromName(string typeName, Func<string, Type> fallback)
    {
        return _nameToTypeCache.GetOrAdd(typeName, fallback);
    }
}