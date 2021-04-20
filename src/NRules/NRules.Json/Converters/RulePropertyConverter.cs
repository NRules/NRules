using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;

namespace NRules.Json.Converters
{
    internal class RulePropertyConverter : JsonConverter<RuleProperty>
    {
        public override RuleProperty Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName &&
                reader.GetString() != nameof(RuleProperty.Name)) throw new JsonException();

            reader.Read();
            string name = reader.GetString();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName &&
                reader.GetString() != "Type") throw new JsonException();
            
            reader.Read();
            var type = JsonSerializer.Deserialize<Type>(ref reader, options);

            reader.Read();
            var value = JsonSerializer.Deserialize(ref reader, type, options);

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject) throw new JsonException();

            return new RuleProperty(name, value);
        }

        public override void Write(Utf8JsonWriter writer, RuleProperty value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(RuleProperty.Name), value.Name);
            
            var type = value.Value is Type ? typeof(Type) : value.Value.GetType();
            writer.WritePropertyName("Type");
            JsonSerializer.Serialize(writer, type, options);
            
            writer.WritePropertyName(nameof(RuleProperty.Value));
            JsonSerializer.Serialize(writer, value.Value, type, options);
            writer.WriteEndObject();
        }
    }
}
