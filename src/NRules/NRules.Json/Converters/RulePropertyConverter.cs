using System;
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
        var type = reader.ReadProperty<Type>("Type", options);
        var value = reader.ReadProperty(nameof(RuleProperty.Value), type, options);
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
