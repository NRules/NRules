using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Json.Converters
{
    internal class RuleDefinitionConverter : JsonConverter<IRuleDefinition>
    {
        public override IRuleDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            string name = default;
            string description = default;
            int priority = default;
            var repeatability = RuleRepeatability.Repeatable;
            var tags = new List<string>();
            var properties = new List<RuleProperty>();
            var dependencies = new List<DependencyElement>();
            var filters = new List<FilterElement>();
            var lhs = default(GroupElement);
            var actions = new List<ActionElement>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

                string propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(IRuleDefinition.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(IRuleDefinition.Description))
                {
                    description = reader.GetString();
                }
                else if (propertyName == nameof(IRuleDefinition.Priority))
                {
                    priority = reader.GetInt32();
                }
                else if (propertyName == nameof(IRuleDefinition.Repeatability))
                {
                    Enum.TryParse(reader.GetString(), out repeatability);
                }
                else if (propertyName == nameof(IRuleDefinition.Tags))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        tags.Add(reader.GetString());
                    }
                }
                else if (propertyName == nameof(IRuleDefinition.Properties))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        properties.Add(JsonSerializer.Deserialize<RuleProperty>(ref reader, options));
                    }
                }
                else if (propertyName == nameof(IRuleDefinition.LeftHandSide))
                {
                    lhs = JsonSerializer.Deserialize<GroupElement>(ref reader, options);
                }
                else if (propertyName == nameof(DependencyGroupElement.Dependencies))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var dependency = JsonSerializer.Deserialize<DependencyElement>(ref reader, options);
                        dependencies.Add(dependency);
                    }
                }
                else if (propertyName == nameof(FilterGroupElement.Filters))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var filter = JsonSerializer.Deserialize<FilterElement>(ref reader, options);
                        filters.Add(filter);
                    }
                }
                else if (propertyName == nameof(IRuleDefinition.RightHandSide))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var action = JsonSerializer.Deserialize<ActionElement>(ref reader, options);
                        actions.Add(action);
                    }
                }
            }

            var ruleDefinition = Element.RuleDefinition(name, description, priority, 
                repeatability, tags, properties, Element.DependencyGroup(dependencies),
                lhs, Element.FilterGroup(filters), Element.ActionGroup(actions));
            return ruleDefinition;
        }

        public override void Write(Utf8JsonWriter writer, IRuleDefinition value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(IRuleDefinition.Name), value.Name);
            if (!string.IsNullOrEmpty(value.Description))
                writer.WriteString(nameof(IRuleDefinition.Description), value.Description);
            writer.WriteNumber(nameof(IRuleDefinition.Priority), value.Priority);
            writer.WriteString(nameof(IRuleDefinition.Repeatability), value.Repeatability.ToString());

            if (value.Tags.Any())
            {
                writer.WriteStartArray(nameof(IRuleDefinition.Tags));
                foreach (var tag in value.Tags)
                {
                    writer.WriteStringValue(tag);
                }
                writer.WriteEndArray();
            }

            if (value.Properties.Any())
            {
                writer.WriteStartArray(nameof(IRuleDefinition.Properties));
                foreach (var property in value.Properties)
                {
                    JsonSerializer.Serialize(writer, property, options);
                }
                writer.WriteEndArray();
            }

            if (value.DependencyGroup.Dependencies.Any())
            {
                writer.WriteStartArray(nameof(DependencyGroupElement.Dependencies));
                foreach (var dependencyElement in value.DependencyGroup.Dependencies)
                {
                    JsonSerializer.Serialize(writer, dependencyElement, options);
                }
                writer.WriteEndArray();
            }

            writer.WritePropertyName(nameof(IRuleDefinition.LeftHandSide));
            JsonSerializer.Serialize(writer, value.LeftHandSide, options);
            
            if (value.FilterGroup.Filters.Any())
            {
                writer.WriteStartArray(nameof(FilterGroupElement.Filters));
                foreach (var filterElement in value.FilterGroup.Filters)
                {
                    JsonSerializer.Serialize(writer, filterElement, options);
                }
                writer.WriteEndArray();
            }
            
            writer.WriteStartArray(nameof(IRuleDefinition.RightHandSide));
            foreach (var actionElement in value.RightHandSide.Actions)
            {
                JsonSerializer.Serialize(writer, actionElement, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
