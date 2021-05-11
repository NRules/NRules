using System;
using System.Collections.Generic;

namespace NRules.Json
{
    /// <summary>
    /// Defines the methods that enable conversion of CLR types to type names
    /// and types names to CLR types for the purpose of JSON serialization.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Gets the name of the type from the CLR type, for the purpose of JSON serialization.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <returns>String representation of the CLR type.</returns>
        string GetTypeName(Type type);

        /// <summary>
        /// Gets the CLR types that corresponds to the type name, retrieved from the JSON document.
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
        private readonly Dictionary<string, Type> _aliasToTypeMap = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _typeToAliasMap = new Dictionary<Type, string>();

        /// <summary><inheritdoc/></summary>
        public string GetTypeName(Type type)
        {
            if (_typeToAliasMap.TryGetValue(type, out var alias))
                return alias;
            return type.AssemblyQualifiedName;
        }
        
        /// <summary><inheritdoc/></summary>
        public Type GetTypeFromName(string typeName)
        {
            if (_aliasToTypeMap.TryGetValue(typeName, out var type))
                return type;
            type = Type.GetType(typeName!);
            if (type == null)
                throw new ArgumentException($"Unrecognized type name. Type={typeName}");
            return type;
        }

        /// <summary>
        /// Registers an alias for a CLR type for the purpose of JSON serialization.
        /// </summary>
        /// <param name="alias">String representation of a CLR type.</param>
        /// <param name="type">CLR type.</param>
        public void RegisterAlias(string alias, Type type)
        {
            _aliasToTypeMap[alias] = type;
            _typeToAliasMap[type] = alias;
        }

        /// <summary>
        /// Registers built-in C# types as aliases for rule serialization.
        /// </summary>
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
}
