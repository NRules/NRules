using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

using static NRules.Json.JsonUtilities;

namespace NRules.Json.Converters
{
    internal class NamedExpressionElementConverter : JsonConverter<NamedExpressionElement>
    {
        public override NamedExpressionElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            string name = default;
            LambdaExpression expression = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(NamedExpressionElement.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(NamedExpressionElement.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<LambdaExpression>(ref reader, options);
                }
            }

            return Element.Expression(name, expression);
        }

        public override void Write(Utf8JsonWriter writer, NamedExpressionElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(JsonName(nameof(NamedExpressionElement.Name), options), value.Name);
            writer.WritePropertyName(JsonName(nameof(NamedExpressionElement.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);
            writer.WriteEndObject();
        }
    }
    
    internal class ActionElementConverter : JsonConverter<ActionElement>
    {
        public override ActionElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            ActionTrigger trigger = ActionElement.DefaultTrigger;
            LambdaExpression expression = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(ActionElement.ActionTrigger), options))
                {
                    Enum.TryParse(reader.GetString(), out trigger);
                }
                else if (JsonNameEquals(propertyName, nameof(ActionElement.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<LambdaExpression>(ref reader, options);
                }
            }

            return Element.Action(expression, trigger);
        }

        public override void Write(Utf8JsonWriter writer, ActionElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.ActionTrigger != ActionElement.DefaultTrigger)
                writer.WriteString(JsonName(nameof(ActionElement.ActionTrigger), options), value.ActionTrigger.ToString()); 
            writer.WritePropertyName(JsonName(nameof(ActionElement.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);
            writer.WriteEndObject();
        }
    }
}
