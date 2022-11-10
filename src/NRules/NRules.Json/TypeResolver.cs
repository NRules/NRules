using System;
using System.Reflection;
using NRules.Json.Utilities;

namespace NRules.Json;

/// <summary>
/// Defines the methods that enable conversion of CLR types to type names
/// and type names to CLR types for the purpose of JSON serialization.
/// </summary>
public interface ITypeResolver
{
    /// <summary>
    /// Gets the name of the type from the CLR type, for the purpose of JSON serialization.
    /// </summary>
    /// <param name="type">CLR type.</param>
    /// <returns>String representation of the CLR type.</returns>
    string? GetTypeName(Type type);

    /// <summary>
    /// Gets the CLR type that corresponds to the type name, retrieved from the JSON document.
    /// </summary>
    /// <param name="typeName">String representation of the CLR type.</param>
    /// <returns>CLR type that corresponds to the string representation.</returns>
    Type GetTypeFromName(string typeName);
}

/// <summary>
/// Default <see cref="ITypeResolver"/> that uses assembly-qualified type names
/// and supports type aliases.
/// </summary>
public class TypeResolver : ITypeResolver
{
    private readonly TypeAliasResolver _aliasResolver = new();
    private readonly TypeCache _cache = new();

    /// <summary><inheritdoc/></summary>
    public string? GetTypeName(Type type)
    {
        return _cache.GetTypeName(type, GetAssemblyQualifiedTypeName);
    }

    private string? GetAssemblyQualifiedTypeName(Type type)
    {
        var typeName = InternalGetTypeName(type);
        return TypeNameFormatter.ConstructAssemblyQualifiedTypeName(typeName);
    }

    private TypeName InternalGetTypeName(Type type)
    {
        if (_aliasResolver.TryGetAliasForType(type, out var alias))
            return new TypeName(alias);

        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            var elementTypeName = InternalGetTypeName(elementType!);
            return TypeNameFormatter.ConstructArrayTypeName(elementTypeName, type.GetArrayRank());
        }

        if (type.IsConstructedGenericType)
        {
            var definitionType = type.GetGenericTypeDefinition();
            var definitionTypeName = InternalGetTypeName(definitionType);

            var typeArguments = type.GenericTypeArguments;
            var typeArgumentNames = new TypeName[typeArguments.Length];
            for (var i = 0; i < typeArguments.Length; i++)
            {
                typeArgumentNames[i] = InternalGetTypeName(typeArguments[i]);
            }

            return TypeNameFormatter.ConstructGenericTypeName(definitionTypeName, typeArgumentNames);
        }

        return new TypeName(type);
    }

    /// <summary><inheritdoc/></summary>
    public Type GetTypeFromName(string typeName)
    {
        return _cache.GetTypeFromName(typeName, GetType);
    }

    private Type GetType(string typeName)
    {
        var type = Type.GetType(typeName, null, ResolveType, throwOnError: false, ignoreCase: false);
        if (type == null)
            throw new ArgumentException($"Unrecognized type name. Type={typeName}");
        return type;
    }

    private Type? ResolveType(Assembly? assembly, string typeName, bool ignoreCase)
    {
        if (_aliasResolver.TryGetTypeByAlias(typeName, out var type))
            return type;

        if (assembly != null)
            return assembly.GetType(typeName, throwOnError: false, ignoreCase);
        return Type.GetType(typeName, throwOnError: false, ignoreCase);
    }

    /// <summary>
    /// Registers an alias for a CLR type for the purpose of JSON serialization.
    /// </summary>
    /// <param name="alias">String representation of the CLR type.</param>
    /// <param name="type">CLR type.</param>
    public void RegisterAlias(string alias, Type type)
    {
        _aliasResolver.RegisterAlias(alias, type);
    }

    /// <summary>
    /// Registers built-in C# types as aliases for rule serialization.
    /// </summary>
    public void RegisterDefaultAliases()
    {
        _aliasResolver.RegisterDefaultAliases();
    }
}
