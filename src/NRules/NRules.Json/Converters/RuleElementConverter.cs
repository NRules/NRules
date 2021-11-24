using System;
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
            writer.WriteEnumProperty(nameof(RuleElement.ElementType), value.ElementType, options);

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
