using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

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
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(NamedExpressionElement.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(NamedExpressionElement.Expression))
                {
                    expression = JsonSerializer.Deserialize<LambdaExpression>(ref reader, options);
                }
            }

            return Element.Expression(name, expression);
        }

        public override void Write(Utf8JsonWriter writer, NamedExpressionElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(NamedExpressionElement.Name), value.Name);
            writer.WritePropertyName(nameof(NamedExpressionElement.Expression));
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
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(ActionElement.ActionTrigger))
                {
                    Enum.TryParse(reader.GetString(), out trigger);
                }
                else if (propertyName == nameof(ActionElement.Expression))
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
                writer.WriteString(nameof(ActionElement.ActionTrigger), value.ActionTrigger.ToString()); 
            writer.WritePropertyName(nameof(ActionElement.Expression));
            JsonSerializer.Serialize(writer, value.Expression, options);
            writer.WriteEndObject();
        }
    }
}
