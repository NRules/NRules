using System;
using System.Collections.Generic;

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
        RegisterAlias("bool", typeof(Boolean));
        RegisterAlias("byte", typeof(Byte));
        RegisterAlias("sbyte", typeof(SByte));
        RegisterAlias("char", typeof(Char));
        RegisterAlias("decimal", typeof(Decimal));
        RegisterAlias("double", typeof(Double));
        RegisterAlias("float", typeof(Single));
        RegisterAlias("int", typeof(Int32));
        RegisterAlias("uint", typeof(UInt32));
        RegisterAlias("long", typeof(Int64));
        RegisterAlias("ulong", typeof(UInt64));
        RegisterAlias("short", typeof(Int16));
        RegisterAlias("ushort", typeof(UInt16));
        RegisterAlias("object", typeof(Object));
        RegisterAlias("string", typeof(String));
    }
}