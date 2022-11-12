namespace NRules.Json;

internal class TypeAliasResolver
{
    private readonly Dictionary<string, Type> _aliasToTypeMap = new();
    private readonly Dictionary<Type, string> _typeToAliasMap = new();

    public bool TryGetAliasForType(Type type, out string alias)
    {
        return _typeToAliasMap.TryGetValue(type, out alias);
    }

    public bool TryGetTypeByAlias(string alias, out Type type)
    {
        return _aliasToTypeMap.TryGetValue(alias, out type);
    }

    public void RegisterAlias(string alias, Type type)
    {
        _aliasToTypeMap[alias] = type;
        _typeToAliasMap[type] = alias;
    }

    public void RegisterDefaultAliases()
    {
        RegisterAlias("bool", typeof(bool));
        RegisterAlias("byte", typeof(byte));
        RegisterAlias("sbyte", typeof(sbyte));
        RegisterAlias("char", typeof(char));
        RegisterAlias("decimal", typeof(decimal));
        RegisterAlias("double", typeof(double));
        RegisterAlias("float", typeof(float));
        RegisterAlias("int", typeof(int));
        RegisterAlias("uint", typeof(uint));
        RegisterAlias("long", typeof(long));
        RegisterAlias("ulong", typeof(ulong));
        RegisterAlias("short", typeof(short));
        RegisterAlias("ushort", typeof(ushort));
        RegisterAlias("object", typeof(object));
        RegisterAlias("string", typeof(string));
    }
}