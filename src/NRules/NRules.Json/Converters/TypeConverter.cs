using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRules.Json.Converters;

internal class TypeConverter : JsonConverter<Type>
{
    private readonly ITypeResolver _typeResolver;

    public TypeConverter(ITypeResolver typeResolver)
    {
        _typeResolver = typeResolver;
    }

    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeName = reader.GetString();
        var type = _typeResolver.GetTypeFromName(typeName);
        return type;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        var typeName = _typeResolver.GetTypeName(value);
        writer.WriteStringValue(typeName);
    }
}