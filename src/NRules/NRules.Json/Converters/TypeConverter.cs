using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRules.Json.Converters
{
    internal class TypeConverter : JsonConverter<Type>
    {
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var typeName = reader.GetString();
            var type = Type.GetType(typeName!);
            return type;
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            var typeName = value.AssemblyQualifiedName;
            writer.WriteStringValue(typeName);
        }
    }
}