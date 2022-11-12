using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;
using NRules.RuleModel;

namespace NRules.Json.Converters;

internal class RulePropertyConverter : JsonConverter<RuleProperty>
{
    public override RuleProperty Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();
        var name = reader.ReadStringProperty(nameof(RuleProperty.Name), options);
        if (name is null)
        {
            throw new JsonException($"Failed to read {nameof(RuleProperty.Name)} property value");
        }
        var type = reader.ReadProperty<Type>("Type", options);
        if (type is null)
        {
            throw new JsonException("Failed to read Type property value");
        }

        var value = reader.ReadProperty(nameof(RuleProperty.Value), type, options);
        if (value is null)
        {
            throw new JsonException($"Failed to read {nameof(RuleProperty.Value)} property value");
        }
        return new RuleProperty(name, value);
    }

    public override void Write(Utf8JsonWriter writer, RuleProperty value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteStringProperty(nameof(RuleProperty.Name), value.Name, options);

        var type = value.Value is Type ? typeof(Type) : value.Value.GetType();
        writer.WriteProperty("Type", type, options);

        writer.WriteProperty(nameof(RuleProperty.Value), value.Value, type, options);
        writer.WriteEndObject();
    }
}
