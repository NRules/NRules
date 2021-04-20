using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Json.Converters
{
    internal class RuleElementConverter : JsonConverter<RuleElement>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(RuleElement).IsAssignableFrom(typeToConvert);

        public override RuleElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName && 
                reader.GetString() != nameof(RuleElement.ElementType)) throw new JsonException();
            reader.Read();
            if (!Enum.TryParse(reader.GetString(), out ElementType elementType)) throw new JsonException();

            RuleElement value;
            if (elementType == ElementType.And)
                value = ReadGroup(ref reader, options, GroupType.And);
            else if (elementType == ElementType.Or)
                value = ReadGroup(ref reader, options, GroupType.Or);
            else if (elementType == ElementType.Pattern)
                value = ReadPattern(ref reader, options);
            else if (elementType == ElementType.Dependency)
                value = ReadDependency(ref reader, options);
            else if (elementType == ElementType.Filter)
                value = ReadFilter(ref reader, options);
            else
                throw new NotSupportedException($"Unsupported element type. ElementType={elementType}");

            return value;
        }
        
        public override void Write(Utf8JsonWriter writer, RuleElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(RuleElement.ElementType), value.ElementType.ToString());

            if (value is GroupElement ge)
                WriteGroup(writer, options, ge);
            else if (value is PatternElement pe)
                WritePattern(writer, options, pe);
            else if (value is DependencyElement de)
                WriteDependency(writer, options, de);
            else if (value is FilterElement fe)
                WriteFilter(writer, options, fe);
            else
                throw new NotSupportedException($"Unsupported element type. ElementType={value.ElementType}");

            writer.WriteEndObject();
        }
        
        private RuleElement ReadGroup(ref Utf8JsonReader reader, JsonSerializerOptions options, GroupType groupType)
        {
            var children = new List<RuleElement>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(GroupElement.ChildElements))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var child = JsonSerializer.Deserialize<RuleElement>(ref reader, options);
                        children.Add(child);
                    }
                }
            }

            return Element.Group(groupType, children);
        }

        private static void WriteGroup(Utf8JsonWriter writer, JsonSerializerOptions options, GroupElement value)
        {
            writer.WriteStartArray(nameof(GroupElement.ChildElements));
            foreach (var childElement in value.ChildElements)
            {
                JsonSerializer.Serialize(writer, childElement, options);
            }
            writer.WriteEndArray();
        }

        private RuleElement ReadPattern(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string name = default;
            Type type = default;
            RuleElement source = default;
            var expressions = new List<NamedExpressionElement>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(Declaration.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(Declaration.Type))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (propertyName == nameof(PatternElement.Expressions))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var expression = JsonSerializer.Deserialize<NamedExpressionElement>(ref reader, options);
                        expressions.Add(expression);
                    }
                }
                else if (propertyName == nameof(PatternElement.Source))
                {
                    source = JsonSerializer.Deserialize<RuleElement>(ref reader, options);
                }
            }

            return Element.Pattern(type, name, expressions, source);
        }

        private static void WritePattern(Utf8JsonWriter writer, JsonSerializerOptions options, PatternElement value)
        {
            writer.WriteString(nameof(Declaration.Name), value.Declaration.Name);

            writer.WritePropertyName(nameof(Declaration.Type));
            JsonSerializer.Serialize(writer, value.Declaration.Type, options);

            writer.WritePropertyName(nameof(PatternElement.Expressions));
            writer.WriteStartArray();
            foreach (var expression in value.Expressions)
            {
                JsonSerializer.Serialize(writer, expression, options);
            }
            writer.WriteEndArray();

            if (value.Source != null)
            {
                writer.WritePropertyName(nameof(PatternElement.Source));
                JsonSerializer.Serialize(writer, value.Source, options);
            }
        }

        private static RuleElement ReadDependency(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string name = default;
            Type type = default;
            Type serviceType = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(DependencyElement.Declaration.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(DependencyElement.Declaration.Type))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (propertyName == nameof(DependencyElement.ServiceType))
                {
                    serviceType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            var declaration = Element.Declaration(type, name);
            return Element.Dependency(declaration, serviceType ?? type);
        }
        
        private void WriteDependency(Utf8JsonWriter writer, JsonSerializerOptions options, DependencyElement value)
        {
            writer.WriteString(nameof(value.Declaration.Name), value.Declaration.Name);
            writer.WritePropertyName(nameof(value.Declaration.Type));
            JsonSerializer.Serialize(writer, value.Declaration.Type, options);
            if (value.ServiceType != value.Declaration.Type)
            {
                writer.WritePropertyName(nameof(value.ServiceType));
                JsonSerializer.Serialize(writer, value.ServiceType, options);
            }
        }
        
        private static RuleElement ReadFilter(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            FilterType filterType = default;
            LambdaExpression expression = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(FilterElement.FilterType))
                {
                    Enum.TryParse(reader.GetString(), out filterType);
                }
                else if (propertyName == nameof(FilterElement.Expression))
                {
                    expression = JsonSerializer.Deserialize<LambdaExpression>(ref reader, options);
                }
            }

            return Element.Filter(filterType, expression);
        }
        
        private void WriteFilter(Utf8JsonWriter writer, JsonSerializerOptions options, FilterElement value)
        {
            writer.WriteString(nameof(value.FilterType), value.FilterType.ToString());
            writer.WritePropertyName(nameof(value.Expression));
            JsonSerializer.Serialize(writer, value.Expression, options);
        }
    }
}
