using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal readonly struct MethodRecord
{
    public readonly string Name;
    public readonly Type? DeclaringType;
    public readonly Type[]? GenericTypeArguments;

    public MethodRecord(string name, Type? declaringType, Type[]? genericTypeArguments = null)
    {
        Name = name;
        DeclaringType = declaringType;
        GenericTypeArguments = genericTypeArguments;
    }
}

internal static class MemberExtensions
{
    public static MemberInfo ReadMemberInfo(this ref Utf8JsonReader reader, JsonSerializerOptions options, Type? impliedType = null)
    {
        var memberType = reader.ReadEnumProperty<MemberTypes>(nameof(MemberInfo.MemberType), options);
        var name = reader.ReadStringProperty(nameof(MemberInfo.Name), options);
        reader.TryReadProperty<Type>(nameof(MemberInfo.DeclaringType), options, out var declaringType);

        var type = declaringType ?? impliedType;
        if (type == null)
            throw new ArgumentException($"Unable to determine declaring type for member. Name={name}");

        switch (memberType)
        {
            case MemberTypes.Field:
                return type.GetField(name) ??
                    throw new ArgumentException($"Unknown field. DeclaringType={type}, Name={name}");
            case MemberTypes.Property:
                return type.GetProperty(name) ??
                    throw new ArgumentException($"Unknown property. DeclaringType={type}, Name={name}");
            default:
                throw new NotSupportedException($"MemberType={memberType}");
        }
    }

    public static void WriteMemberInfo(this Utf8JsonWriter writer, JsonSerializerOptions options, MemberInfo value, Type? impliedType = null)
    {
        writer.WriteEnumProperty(nameof(value.MemberType), value.MemberType, options);
        writer.WriteStringProperty(nameof(value.Name), value.Name, options);
        if (impliedType != value.DeclaringType)
            writer.WriteProperty(nameof(value.DeclaringType), value.DeclaringType, options);
    }

    public static MethodRecord ReadMethodInfo(this ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty("MethodName", options);
        reader.TryReadProperty<Type>(nameof(MethodInfo.DeclaringType), options, out var declaringType);
        reader.TryReadArrayProperty<Type>("GenericTypeArguments", options, out var genericTypeArguments);
        var genericTypes = genericTypeArguments.Count > 0 ? genericTypeArguments.ToArray() : null;
        return new MethodRecord(name, declaringType, genericTypes);
    }

    public static bool TryReadMethodInfo(this ref Utf8JsonReader reader, JsonSerializerOptions options, out MethodRecord method)
    {
        method = default;

        if (!reader.TryReadStringProperty("MethodName", options, out var name))
            return false;

        reader.TryReadProperty<Type>(nameof(MethodInfo.DeclaringType), options, out var declaringType);
        reader.TryReadArrayProperty<Type>("GenericTypeArguments", options, out var genericTypeArguments);
        var genericTypes = genericTypeArguments.Count > 0 ? genericTypeArguments.ToArray() : null;
        method = new MethodRecord(name, declaringType, genericTypes);
        return true;
    }

    public static void WriteMethodInfo(this Utf8JsonWriter writer, JsonSerializerOptions options, MethodInfo value, Type? impliedType = null)
    {
        writer.WriteStringProperty("MethodName", value.Name, options);
        if (impliedType != value.DeclaringType)
            writer.WriteProperty(nameof(value.DeclaringType), value.DeclaringType, options);
        if (value.IsGenericMethod)
            writer.WriteArrayProperty("GenericTypeArguments", value.GetGenericArguments(), options);
    }

    public static MethodInfo GetMethod(this MethodRecord methodRecord, Type[] argumentTypes, Type? impliedType = null)
    {
        var type = methodRecord.DeclaringType ?? impliedType;
        if (type == null)
            throw new ArgumentException($"Unable to determine declaring type for method. Name={methodRecord.Name}");

        if (methodRecord.GenericTypeArguments != null)
            return ResolveGenericMethod(type, methodRecord.Name, methodRecord.GenericTypeArguments, argumentTypes);

        var method = type.GetMethod(methodRecord.Name, argumentTypes)
            ?? throw new ArgumentException($"Unknown method. DeclaringType={type}, Name={methodRecord.Name}");
        return method;
    }

    private static MethodInfo ResolveGenericMethod(Type type, string name, Type[] genericTypeArguments, Type[] argumentTypes)
    {
        var candidates = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
            .Where(m => m.Name == name && m.IsGenericMethodDefinition &&
                        m.GetGenericArguments().Length == genericTypeArguments.Length &&
                        m.GetParameters().Length == argumentTypes.Length);

        foreach (var candidate in candidates)
        {
            var constructed = candidate.MakeGenericMethod(genericTypeArguments);
            var parameters = constructed.GetParameters();
            var match = true;
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != argumentTypes[i])
                {
                    match = false;
                    break;
                }
            }
            if (match) return constructed;
        }

        throw new ArgumentException(
            $"Unknown generic method. DeclaringType={type}, Name={name}, " +
            $"GenericTypeArguments=[{string.Join(", ", genericTypeArguments.AsEnumerable())}], " +
            $"ArgumentTypes=[{string.Join(", ", argumentTypes.AsEnumerable())}]");
    }
}
