using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

using static NRules.Json.JsonUtilities;

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
                !JsonNameConvertEquals(nameof(RuleElement.ElementType), reader.GetString(), options)) throw new JsonException();
            reader.Read();
            if (!Enum.TryParse(reader.GetString(), out ElementType elementType)) throw new JsonException();

            RuleElement value;
            if (elementType == ElementType.And)
                value = ReadGroup(ref reader, options, GroupType.And);
            else if (elementType == ElementType.Or)
                value = ReadGroup(ref reader, options, GroupType.Or);
            else if (elementType == ElementType.Exists)
                value = ReadExists(ref reader, options);
            else if (elementType == ElementType.Not)
                value = ReadNot(ref reader, options);
            else if (elementType == ElementType.Aggregate)
                value = ReadAggregate(ref reader, options);
            else if (elementType == ElementType.Binding)
                value = ReadBinding(ref reader, options);
            else if (elementType == ElementType.Pattern)
                value = ReadPattern(ref reader, options);
            else if (elementType == ElementType.Dependency)
                value = ReadDependency(ref reader, options);
            else if (elementType == ElementType.Filter)
                value = ReadFilter(ref reader, options);
            else if (elementType == ElementType.ForAll)
                value = ReadForAll(ref reader, options);
            else
                throw new NotSupportedException($"Unsupported element type. ElementType={elementType}");

            return value;
        }
        
        public override void Write(Utf8JsonWriter writer, RuleElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(JsonName(nameof(RuleElement.ElementType), options), value.ElementType.ToString());

            if (value is GroupElement ge)
                WriteGroup(writer, options, ge);
            else if (value is ExistsElement ee)
                WriteExists(writer, options, ee);
            else if (value is NotElement ne)
                WriteNot(writer, options, ne);
            else if (value is AggregateElement ae)
                WriteAggregate(writer, options, ae);
            else if (value is BindingElement be)
                WriteBinding(writer, options, be);
            else if (value is PatternElement pe)
                WritePattern(writer, options, pe);
            else if (value is DependencyElement de)
                WriteDependency(writer, options, de);
            else if (value is FilterElement fe)
                WriteFilter(writer, options, fe);
            else if (value is ForAllElement fa)
                WriteForAll(writer, options, fa);
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
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(GroupElement.ChildElements), options))
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
            writer.WriteStartArray(JsonName(nameof(GroupElement.ChildElements), options));
            foreach (var childElement in value.ChildElements)
            {
                JsonSerializer.Serialize(writer, childElement, options);
            }
            writer.WriteEndArray();
        }

        private RuleElement ReadExists(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            RuleElement source = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(ExistsElement.Source), options))
                {
                    source = JsonSerializer.Deserialize<RuleElement>(ref reader, options);
                }
            }

            return Element.Exists(source);
        }

        private static void WriteExists(Utf8JsonWriter writer, JsonSerializerOptions options, ExistsElement value)
        {
            writer.WritePropertyName(JsonName(nameof(ExistsElement.Source), options));
            JsonSerializer.Serialize(writer, value.Source, options);
        }

        private RuleElement ReadNot(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            RuleElement source = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(NotElement.Source), options))
                {
                    source = JsonSerializer.Deserialize<RuleElement>(ref reader, options);
                }
            }

            return Element.Not(source);
        }

        private static void WriteNot(Utf8JsonWriter writer, JsonSerializerOptions options, NotElement value)
        {
            writer.WritePropertyName(JsonName(nameof(NotElement.Source), options));
            JsonSerializer.Serialize(writer, value.Source, options);
        }
        
        private RuleElement ReadAggregate(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string name = default;
            Type resultType = default;
            var expressions = new List<NamedExpressionElement>();
            PatternElement source = default;
            Type customFactoryType = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(AggregateElement.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(AggregateElement.ResultType), options))
                {
                    resultType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(PatternElement.Expressions), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var expression = JsonSerializer.Deserialize<NamedExpressionElement>(ref reader, options);
                        expressions.Add(expression);
                    }
                }
                else if (JsonNameEquals(propertyName, nameof(AggregateElement.Source), options))
                {
                    source = JsonSerializer.Deserialize<PatternElement>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(AggregateElement.CustomFactoryType), options))
                {
                    customFactoryType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            return Element.Aggregate(resultType, name, expressions, source, customFactoryType);
        }

        private static void WriteAggregate(Utf8JsonWriter writer, JsonSerializerOptions options, AggregateElement value)
        {
            writer.WriteString(JsonName(nameof(AggregateElement.Name), options), value.Name);
            writer.WritePropertyName(JsonName(nameof(AggregateElement.ResultType), options));
            JsonSerializer.Serialize(writer, value.ResultType, options);

            if (value.Expressions.Any())
            {
                writer.WritePropertyName(JsonName(nameof(AggregateElement.Expressions), options));
                writer.WriteStartArray();
                foreach (var expression in value.Expressions)
                {
                    JsonSerializer.Serialize(writer, expression, options);
                }
                writer.WriteEndArray();
            }

            if (value.CustomFactoryType != null)
            {
                writer.WritePropertyName(JsonName(nameof(AggregateElement.CustomFactoryType), options));
                JsonSerializer.Serialize(writer, value.CustomFactoryType, options);
            }

            writer.WritePropertyName(JsonName(nameof(AggregateElement.Source), options));
            JsonSerializer.Serialize(writer, value.Source, options);
        }

        private RuleElement ReadBinding(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Type resultType = default;
            LambdaExpression expression = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(BindingElement.ResultType), options))
                {
                    resultType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(BindingElement.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<LambdaExpression>(ref reader, options);
                }
            }

            return Element.Binding(resultType, expression);
        }

        private static void WriteBinding(Utf8JsonWriter writer, JsonSerializerOptions options, BindingElement value)
        {
            writer.WritePropertyName(JsonName(nameof(BindingElement.ResultType), options));
            JsonSerializer.Serialize(writer, value.ResultType, options);

            writer.WritePropertyName(JsonName(nameof(BindingElement.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);
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
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(Declaration.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(Declaration.Type), options))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(PatternElement.Expressions), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var expression = JsonSerializer.Deserialize<NamedExpressionElement>(ref reader, options);
                        expressions.Add(expression);
                    }
                }
                else if (JsonNameEquals(propertyName, nameof(PatternElement.Source), options))
                {
                    source = JsonSerializer.Deserialize<RuleElement>(ref reader, options);
                }
            }

            return Element.Pattern(type, name, expressions, source);
        }

        private static void WritePattern(Utf8JsonWriter writer, JsonSerializerOptions options, PatternElement value)
        {
            writer.WriteString(JsonName(nameof(Declaration.Name), options), value.Declaration.Name);

            writer.WritePropertyName(JsonName(nameof(Declaration.Type), options));
            JsonSerializer.Serialize(writer, value.Declaration.Type, options);

            if (value.Expressions.Any())
            {
                writer.WritePropertyName(JsonName(nameof(PatternElement.Expressions), options));
                writer.WriteStartArray();
                foreach (var expression in value.Expressions)
                {
                    JsonSerializer.Serialize(writer, expression, options);
                }

                writer.WriteEndArray();
            }

            if (value.Source != null)
            {
                writer.WritePropertyName(JsonName(nameof(PatternElement.Source), options));
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
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(DependencyElement.Declaration.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(DependencyElement.Declaration.Type), options))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(DependencyElement.ServiceType), options))
                {
                    serviceType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            var declaration = Element.Declaration(type, name);
            return Element.Dependency(declaration, serviceType ?? type);
        }
        
        private void WriteDependency(Utf8JsonWriter writer, JsonSerializerOptions options, DependencyElement value)
        {
            writer.WriteString(JsonName(nameof(value.Declaration.Name), options), value.Declaration.Name);
            writer.WritePropertyName(JsonName(nameof(value.Declaration.Type), options));
            JsonSerializer.Serialize(writer, value.Declaration.Type, options);
            if (value.ServiceType != value.Declaration.Type)
            {
                writer.WritePropertyName(JsonName(nameof(value.ServiceType), options));
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
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(FilterElement.FilterType), options))
                {
                    Enum.TryParse(reader.GetString(), out filterType);
                }
                else if (JsonNameEquals(propertyName, nameof(FilterElement.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<LambdaExpression>(ref reader, options);
                }
            }

            return Element.Filter(filterType, expression);
        }
        
        private void WriteFilter(Utf8JsonWriter writer, JsonSerializerOptions options, FilterElement value)
        {
            writer.WriteString(JsonName(nameof(value.FilterType), options), value.FilterType.ToString());
            writer.WritePropertyName(JsonName(nameof(value.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);
        }

        private RuleElement ReadForAll(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            PatternElement basePattern = default;
            var patterns = new List<PatternElement>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(ForAllElement.BasePattern), options))
                {
                    basePattern = JsonSerializer.Deserialize<PatternElement>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(ForAllElement.Patterns), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var pattern = JsonSerializer.Deserialize<PatternElement>(ref reader, options);
                        patterns.Add(pattern);
                    }
                }
            }

            return Element.ForAll(basePattern, patterns);
        }

        private static void WriteForAll(Utf8JsonWriter writer, JsonSerializerOptions options, ForAllElement value)
        {
            writer.WritePropertyName(JsonName(nameof(ForAllElement.BasePattern), options));
            JsonSerializer.Serialize(writer, value.BasePattern, options);

            writer.WritePropertyName(JsonName(nameof(ForAllElement.Patterns), options));
            writer.WriteStartArray();
            foreach (var pattern in value.Patterns)
            {
                JsonSerializer.Serialize(writer, pattern, options);
            }

            writer.WriteEndArray();
        }
    }
}
