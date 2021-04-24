using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

using static NRules.Json.JsonUtilities;

namespace NRules.Json.Converters
{
    internal class RuleDefinitionConverter : JsonConverter<IRuleDefinition>
    {
        public override IRuleDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            string name = default;
            string description = string.Empty;
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

                string propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(IRuleDefinition.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.Description), options))
                {
                    description = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.Priority), options))
                {
                    priority = reader.GetInt32();
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.Repeatability), options))
                {
                    Enum.TryParse(reader.GetString(), out repeatability);
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.Tags), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        tags.Add(reader.GetString());
                    }
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.Properties), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        properties.Add(JsonSerializer.Deserialize<RuleProperty>(ref reader, options));
                    }
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.LeftHandSide), options))
                {
                    lhs = JsonSerializer.Deserialize<GroupElement>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(DependencyGroupElement.Dependencies), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var dependency = JsonSerializer.Deserialize<DependencyElement>(ref reader, options);
                        dependencies.Add(dependency);
                    }
                }
                else if (JsonNameEquals(propertyName, nameof(FilterGroupElement.Filters), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
                    
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var filter = JsonSerializer.Deserialize<FilterElement>(ref reader, options);
                        filters.Add(filter);
                    }
                }
                else if (JsonNameEquals(propertyName, nameof(IRuleDefinition.RightHandSide), options))
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

            writer.WriteString(JsonName(nameof(IRuleDefinition.Name), options), value.Name);
            if (!string.IsNullOrEmpty(value.Description))
                writer.WriteString(JsonName(nameof(IRuleDefinition.Description), options), value.Description);
            writer.WriteNumber(JsonName(nameof(IRuleDefinition.Priority), options), value.Priority);
            writer.WriteString(JsonName(nameof(IRuleDefinition.Repeatability), options), value.Repeatability.ToString());

            if (value.Tags.Any())
            {
                writer.WriteStartArray(JsonName(nameof(IRuleDefinition.Tags), options));
                foreach (var tag in value.Tags)
                {
                    writer.WriteStringValue(tag);
                }
                writer.WriteEndArray();
            }

            if (value.Properties.Any())
            {
                writer.WriteStartArray(JsonName(nameof(IRuleDefinition.Properties), options));
                foreach (var property in value.Properties)
                {
                    JsonSerializer.Serialize(writer, property, options);
                }
                writer.WriteEndArray();
            }

            if (value.DependencyGroup.Dependencies.Any())
            {
                writer.WriteStartArray(JsonName(nameof(DependencyGroupElement.Dependencies), options));
                foreach (var dependencyElement in value.DependencyGroup.Dependencies)
                {
                    JsonSerializer.Serialize(writer, dependencyElement, options);
                }
                writer.WriteEndArray();
            }

            writer.WritePropertyName(JsonName(nameof(IRuleDefinition.LeftHandSide), options));
            JsonSerializer.Serialize(writer, value.LeftHandSide, options);
            
            if (value.FilterGroup.Filters.Any())
            {
                writer.WriteStartArray(JsonName(nameof(FilterGroupElement.Filters), options));
                foreach (var filterElement in value.FilterGroup.Filters)
                {
                    JsonSerializer.Serialize(writer, filterElement, options);
                }
                writer.WriteEndArray();
            }
            
            writer.WriteStartArray(JsonName(nameof(IRuleDefinition.RightHandSide), options));
            foreach (var actionElement in value.RightHandSide.Actions)
            {
                JsonSerializer.Serialize(writer, actionElement, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
