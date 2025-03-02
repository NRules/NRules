using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRules.Json.Converters;

internal class TypeConverter(ITypeResolver typeResolver) : JsonConverter<Type>
{
    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeName = reader.GetString()
            ?? throw new JsonException("Expected string value");
        var type = typeResolver.GetTypeFromName(typeName);
        return type;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        var typeName = typeResolver.GetTypeName(value);
        writer.WriteStringValue(typeName);
    }
}