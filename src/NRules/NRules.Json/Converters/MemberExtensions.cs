using System;
using System.Reflection;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal readonly struct MethodRecord
{
    public readonly string Name;
    public readonly Type? DeclaringType;

    public MethodRecord(string name, Type? declaringType)
    {
        Name = name;
        DeclaringType = declaringType;
    }
}

internal static class MemberExtensions
{
    public static MemberInfo ReadMemberInfo(this ref Utf8JsonReader reader, JsonSerializerOptions options, Type? impliedType = null)
    {
        var memberType = reader.ReadEnumProperty<MemberTypes>(nameof(MemberInfo.MemberType), options);
        var name = reader.ReadStringProperty(nameof(MemberInfo.Name), options)
            ?? throw new JsonException($"Property '{nameof(MemberInfo.Name)}' should have not null value");
        reader.TryReadProperty<Type>(nameof(MemberInfo.DeclaringType), options, out var declaringType);

        var type = DetermineType(declaringType, impliedType, name);

        switch (memberType)
        {
            case MemberTypes.Field:
                return type.GetField(name) ??
                    throw new ArgumentException($"Unknown field. DeclaringType={type}, Name={name}", nameof(name));
            case MemberTypes.Property:
                return type.GetProperty(name) ??
                    throw new ArgumentException($"Unknown property. DeclaringType={type}, Name={name}", nameof(name));
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
        var name = reader.ReadStringProperty("MethodName", options)
            ?? throw new JsonException("Property 'MethodName' should have not null value");
        reader.TryReadProperty<Type>(nameof(MethodInfo.DeclaringType), options, out var declaringType);
        return new MethodRecord(name, declaringType);
    }

    public static bool TryReadMethodInfo(this ref Utf8JsonReader reader, JsonSerializerOptions options, out MethodRecord method)
    {
        method = default;

        if (!reader.TryReadStringProperty("MethodName", options, out var name) || name is null)
            return false;

        reader.TryReadProperty<Type>(nameof(MethodInfo.DeclaringType), options, out var declaringType);
        method = new MethodRecord(name, declaringType);
        return true;
    }

    public static void WriteMethodInfo(this Utf8JsonWriter writer, JsonSerializerOptions options, MethodInfo value, Type? impliedType = null)
    {
        writer.WriteStringProperty("MethodName", value.Name, options);
        if (impliedType != value.DeclaringType)
            writer.WriteProperty(nameof(value.DeclaringType), value.DeclaringType, options);
    }

    public static MethodInfo GetMethod(this MethodRecord methodRecord, Type[] argumentTypes, Type? impliedType = null)
    {
        var type = DetermineType(methodRecord.DeclaringType, impliedType, methodRecord.Name);

        var method = type.GetMethod(methodRecord.Name, argumentTypes)
            ?? throw new ArgumentException($"Unknown method. DeclaringType={type}, Name={methodRecord.Name}");
        return method;
    }

    private static Type DetermineType(Type? declaringType, Type? impliedType, string memberName)
    {
        return declaringType ?? impliedType ?? throw new ArgumentException($"Unable to determine declaring type for member. Name={memberName}");
    }
}
