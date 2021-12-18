﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;
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
            reader.ReadStartObject();
            var elementType = reader.ReadEnumProperty<ElementType>(nameof(RuleElement.ElementType), options);

            switch (elementType)
            {
                case ElementType.And:
                    return ReadGroup(ref reader, options, GroupType.And);
                case ElementType.Or:
                    return ReadGroup(ref reader, options, GroupType.Or);
                case ElementType.Exists:
                    return ReadExists(ref reader, options);
                case ElementType.Not:
                    return ReadNot(ref reader, options);
                case ElementType.Aggregate:
                    return ReadAggregate(ref reader, options);
                case ElementType.Binding:
                    return ReadBinding(ref reader, options);
                case ElementType.Pattern:
                    return ReadPattern(ref reader, options);
                case ElementType.Dependency:
                    return ReadDependency(ref reader, options);
                case ElementType.Filter:
                    return ReadFilter(ref reader, options);
                case ElementType.ForAll:
                    return ReadForAll(ref reader, options);
                default:
                    throw new NotSupportedException($"Unsupported element type. ElementType={elementType}");
            }
        }
        
        public override void Write(Utf8JsonWriter writer, RuleElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteEnumProperty(nameof(RuleElement.ElementType), value.ElementType, options);

            switch (value)
            {
                case GroupElement ge:
                    WriteGroup(writer, options, ge);
                    break;
                case ExistsElement ee:
                    WriteExists(writer, options, ee);
                    break;
                case NotElement ne:
                    WriteNot(writer, options, ne);
                    break;
                case AggregateElement ae:
                    WriteAggregate(writer, options, ae);
                    break;
                case BindingElement be:
                    WriteBinding(writer, options, be);
                    break;
                case PatternElement pe:
                    WritePattern(writer, options, pe);
                    break;
                case DependencyElement de:
                    WriteDependency(writer, options, de);
                    break;
                case FilterElement fe:
                    WriteFilter(writer, options, fe);
                    break;
                case ForAllElement fa:
                    WriteForAll(writer, options, fa);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported element type. ElementType={value.ElementType}");
            }

            writer.WriteEndObject();
        }
        
        private GroupElement ReadGroup(ref Utf8JsonReader reader, JsonSerializerOptions options, GroupType groupType)
        {
            var children = reader.ReadArrayProperty<RuleElement>(nameof(GroupElement.ChildElements), options);
            return Element.Group(groupType, children);
        }

        private static void WriteGroup(Utf8JsonWriter writer, JsonSerializerOptions options, GroupElement value)
        {
            writer.WriteArrayProperty(nameof(GroupElement.ChildElements), value.ChildElements, options);
        }

        private ExistsElement ReadExists(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var source = reader.ReadProperty<RuleElement>(nameof(ExistsElement.Source), options);
            return Element.Exists(source);
        }

        private static void WriteExists(Utf8JsonWriter writer, JsonSerializerOptions options, ExistsElement value)
        {
            writer.WriteProperty(nameof(ExistsElement.Source), value.Source, options);
        }

        private NotElement ReadNot(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var source = reader.ReadProperty<RuleElement>(nameof(NotElement.Source), options);
            return Element.Not(source);
        }

        private static void WriteNot(Utf8JsonWriter writer, JsonSerializerOptions options, NotElement value)
        {
            writer.WriteProperty(nameof(NotElement.Source), value.Source, options);
        }
        
        private AggregateElement ReadAggregate(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var name = reader.ReadStringProperty(nameof(AggregateElement.Name), options);
            var resultType = reader.ReadProperty<Type>(nameof(AggregateElement.ResultType), options);
            reader.TryReadArrayProperty<NamedExpressionElement>(nameof(AggregateElement.Expressions), options, out var expressions);
            reader.TryReadProperty<Type>(nameof(AggregateElement.CustomFactoryType), options, out var customFactoryType);
            var source = reader.ReadProperty<PatternElement>(nameof(AggregateElement.Source), options);
            return Element.Aggregate(resultType, name, expressions, source, customFactoryType);
        }

        private static void WriteAggregate(Utf8JsonWriter writer, JsonSerializerOptions options, AggregateElement value)
        {
            writer.WriteStringProperty(nameof(AggregateElement.Name), value.Name, options);
            writer.WriteProperty(nameof(AggregateElement.ResultType), value.ResultType, options);

            if (value.Expressions.Any())
                writer.WriteArrayProperty(nameof(AggregateElement.Expressions), value.Expressions, options);

            if (value.CustomFactoryType != null)
                writer.WriteProperty(nameof(AggregateElement.CustomFactoryType), value.CustomFactoryType, options);

            writer.WriteProperty(nameof(AggregateElement.Source), value.Source, options);
        }

        private BindingElement ReadBinding(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var resultType = reader.ReadProperty<Type>(nameof(BindingElement.ResultType), options);
            var expression = reader.ReadProperty<LambdaExpression>(nameof(BindingElement.Expression), options);
            return Element.Binding(resultType, expression);
        }

        private static void WriteBinding(Utf8JsonWriter writer, JsonSerializerOptions options, BindingElement value)
        {
            writer.WriteProperty(nameof(BindingElement.ResultType), value.ResultType, options);
            writer.WriteProperty(nameof(BindingElement.Expression), value.Expression, options);
        }

        private PatternElement ReadPattern(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var name = reader.ReadStringProperty(nameof(Declaration.Name), options);
            var type = reader.ReadProperty<Type>(nameof(Declaration.Type), options);
            reader.TryReadArrayProperty<NamedExpressionElement>(nameof(PatternElement.Expressions), options, out var expressions);
            reader.TryReadProperty<RuleElement>(nameof(PatternElement.Source), options, out var source);
            return Element.Pattern(type, name, expressions, source);
        }

        private static void WritePattern(Utf8JsonWriter writer, JsonSerializerOptions options, PatternElement value)
        {
            writer.WriteStringProperty(nameof(Declaration.Name), value.Declaration.Name, options);
            writer.WriteProperty(nameof(Declaration.Type), value.Declaration.Type, options);

            if (value.Expressions.Any())
                writer.WriteArrayProperty(nameof(PatternElement.Expressions), value.Expressions, options);

            if (value.Source != null)
                writer.WriteProperty(nameof(PatternElement.Source), value.Source, options);
        }

        private static DependencyElement ReadDependency(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var name = reader.ReadStringProperty(nameof(DependencyElement.Declaration.Name), options);
            var type = reader.ReadProperty<Type>(nameof(DependencyElement.Declaration.Type), options);
            reader.TryReadProperty<Type>(nameof(DependencyElement.ServiceType), options, out var serviceType);
            var declaration = Element.Declaration(type, name);
            return Element.Dependency(declaration, serviceType ?? type);
        }
        
        private void WriteDependency(Utf8JsonWriter writer, JsonSerializerOptions options, DependencyElement value)
        {
            writer.WriteStringProperty(nameof(Declaration.Name), value.Declaration.Name, options);
            writer.WriteProperty(nameof(Declaration.Type), value.Declaration.Type, options);

            if (value.ServiceType != value.Declaration.Type)
                writer.WriteProperty(nameof(value.ServiceType), value.ServiceType, options);
        }
        
        private static FilterElement ReadFilter(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var filterType = reader.ReadEnumProperty<FilterType>(nameof(FilterElement.FilterType), options);
            var expression = reader.ReadProperty<LambdaExpression>(nameof(FilterElement.Expression), options);
            return Element.Filter(filterType, expression);
        }
        
        private void WriteFilter(Utf8JsonWriter writer, JsonSerializerOptions options, FilterElement value)
        {
            writer.WriteEnumProperty(nameof(value.FilterType), value.FilterType, options);
            writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        }

        private ForAllElement ReadForAll(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var basePattern = reader.ReadProperty<PatternElement>(nameof(ForAllElement.BasePattern), options);
            var patterns = reader.ReadArrayProperty<PatternElement>(nameof(ForAllElement.Patterns), options);
            return Element.ForAll(basePattern, patterns);
        }

        private static void WriteForAll(Utf8JsonWriter writer, JsonSerializerOptions options, ForAllElement value)
        {
            writer.WriteProperty(nameof(ForAllElement.BasePattern), value.BasePattern, options);
            writer.WriteArrayProperty(nameof(ForAllElement.Patterns), value.Patterns, options);
        }
    }
}
