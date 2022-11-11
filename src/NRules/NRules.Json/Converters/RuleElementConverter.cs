using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Json.Converters;

internal class RuleElementConverter : JsonConverter<RuleElement>
{
    public override bool CanConvert(Type typeToConvert) =>
        typeof(RuleElement).IsAssignableFrom(typeToConvert);

    public override RuleElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();
        var elementType = reader.ReadEnumProperty<ElementType>(nameof(RuleElement.ElementType), options);

        return elementType switch
        {
            ElementType.And => ReadGroup(ref reader, options, GroupType.And),
            ElementType.Or => ReadGroup(ref reader, options, GroupType.Or),
            ElementType.Exists => ReadExists(ref reader, options),
            ElementType.Not => ReadNot(ref reader, options),
            ElementType.Aggregate => ReadAggregate(ref reader, options),
            ElementType.Binding => ReadBinding(ref reader, options),
            ElementType.Pattern => ReadPattern(ref reader, options),
            ElementType.Dependency => ReadDependency(ref reader, options),
            ElementType.Filter => ReadFilter(ref reader, options),
            ElementType.ForAll => ReadForAll(ref reader, options),
            _ => throw new NotSupportedException($"Unsupported element type. ElementType={elementType}")
        };
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

    private static GroupElement ReadGroup(ref Utf8JsonReader reader, JsonSerializerOptions options, GroupType groupType)
    {
        var children = reader.ReadArrayProperty<RuleElement>(nameof(GroupElement.ChildElements), options);
        return Element.Group(groupType, children);
    }

    private static void WriteGroup(Utf8JsonWriter writer, JsonSerializerOptions options, GroupElement value)
    {
        writer.WriteArrayProperty(nameof(GroupElement.ChildElements), value.ChildElements, options);
    }

    private static ExistsElement ReadExists(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var source = reader.ReadProperty<RuleElement>(nameof(ExistsElement.Source), options);
        if (source is null)
        {
            throw new JsonException($"Failed to read {nameof(ExistsElement.Source)} property value");
        }
        return Element.Exists(source);
    }

    private static void WriteExists(Utf8JsonWriter writer, JsonSerializerOptions options, ExistsElement value)
    {
        writer.WriteProperty(nameof(ExistsElement.Source), value.Source, options);
    }

    private static NotElement ReadNot(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var source = reader.ReadProperty<RuleElement>(nameof(NotElement.Source), options);
        if (source is null)
        {
            throw new JsonException($"Failed to read {nameof(NotElement.Source)} property value");
        }
        return Element.Not(source);
    }

    private static void WriteNot(Utf8JsonWriter writer, JsonSerializerOptions options, NotElement value)
    {
        writer.WriteProperty(nameof(NotElement.Source), value.Source, options);
    }

    private static AggregateElement ReadAggregate(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty(nameof(AggregateElement.Name), options);
        if (name is null)
        {
            throw new JsonException($"Failed to read {nameof(AggregateElement.Name)} property value");
        }
        var resultType = reader.ReadProperty<Type>(nameof(AggregateElement.ResultType), options);
        if (resultType is null)
        {
            throw new JsonException($"Failed to read {nameof(AggregateElement.ResultType)} property value");
        }

        reader.TryReadArrayProperty<NamedExpressionElement>(nameof(AggregateElement.Expressions), options, out var expressions);

        reader.TryReadProperty<Type>(nameof(AggregateElement.CustomFactoryType), options, out var customFactoryType);
        var source = reader.ReadProperty<PatternElement>(nameof(AggregateElement.Source), options);
        if (source is null)
        {
            throw new JsonException($"Failed to read {nameof(AggregateElement.Source)} property value");
        }
        return Element.Aggregate(resultType, name, expressions!, source, customFactoryType);
    }

    private static void WriteAggregate(Utf8JsonWriter writer, JsonSerializerOptions options, AggregateElement value)
    {
        writer.WriteStringProperty(nameof(AggregateElement.Name), value.Name, options);
        writer.WriteProperty(nameof(AggregateElement.ResultType), value.ResultType, options);

        if (value.Expressions.Any())
            writer.WriteArrayProperty(nameof(AggregateElement.Expressions), value.Expressions, options);

        if (value.CustomFactoryType is not null)
            writer.WriteProperty(nameof(AggregateElement.CustomFactoryType), value.CustomFactoryType, options);

        writer.WriteProperty(nameof(AggregateElement.Source), value.Source, options);
    }

    private static BindingElement ReadBinding(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var resultType = reader.ReadProperty<Type>(nameof(BindingElement.ResultType), options);
        if (resultType is null)
        {
            throw new JsonException($"Failed to read {nameof(BindingElement.ResultType)} property value");
        }
        var expression = reader.ReadProperty<LambdaExpression>(nameof(BindingElement.Expression), options);
        if (expression is null)
        {
            throw new JsonException($"Failed to read {nameof(BindingElement.Expression)} property value");
        }
        return Element.Binding(resultType, expression);
    }

    private static void WriteBinding(Utf8JsonWriter writer, JsonSerializerOptions options, BindingElement value)
    {
        writer.WriteProperty(nameof(BindingElement.ResultType), value.ResultType, options);
        writer.WriteProperty(nameof(BindingElement.Expression), value.Expression, options);
    }

    private static PatternElement ReadPattern(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty(nameof(Declaration.Name), options);
        if (name is null)
        {
            throw new JsonException($"Failed to read {nameof(Declaration.Name)} property value");
        }
        var type = reader.ReadProperty<Type>(nameof(Declaration.Type), options);
        if (type is null)
        {
            throw new JsonException($"Failed to read {nameof(Declaration.Type)} property value");
        }

        reader.TryReadArrayProperty<NamedExpressionElement>(nameof(PatternElement.Expressions), options, out var expressions);
        reader.TryReadProperty<RuleElement>(nameof(PatternElement.Source), options, out var source);
        return Element.Pattern(type, name, expressions!, source);
    }

    private static void WritePattern(Utf8JsonWriter writer, JsonSerializerOptions options, PatternElement value)
    {
        writer.WriteStringProperty(nameof(Declaration.Name), value.Declaration.Name, options);
        writer.WriteProperty(nameof(Declaration.Type), value.Declaration.Type, options);

        if (value.Expressions.Any())
            writer.WriteArrayProperty(nameof(PatternElement.Expressions), value.Expressions, options);

        if (value.Source is not null)
            writer.WriteProperty(nameof(PatternElement.Source), value.Source, options);
    }

    private static DependencyElement ReadDependency(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty(nameof(DependencyElement.Declaration.Name), options);
        if (name is null)
        {
            throw new JsonException($"Failed to read {nameof(DependencyElement.Declaration.Name)} property value");
        }
        var type = reader.ReadProperty<Type>(nameof(DependencyElement.Declaration.Type), options);
        if (type is null)
        {
            throw new JsonException($"Failed to read {nameof(DependencyElement.Declaration.Type)} property value");
        }

        reader.TryReadProperty<Type>(nameof(DependencyElement.ServiceType), options, out var serviceType);
        var declaration = Element.Declaration(type, name);
        return Element.Dependency(declaration, serviceType ?? type);
    }

    private static void WriteDependency(Utf8JsonWriter writer, JsonSerializerOptions options, DependencyElement value)
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
        if (expression is null)
        {
            throw new JsonException($"Failed to read {nameof(FilterElement.Expression)} property value");
        }
        return Element.Filter(filterType, expression);
    }

    private static void WriteFilter(Utf8JsonWriter writer, JsonSerializerOptions options, FilterElement value)
    {
        writer.WriteEnumProperty(nameof(value.FilterType), value.FilterType, options);
        writer.WriteProperty(nameof(value.Expression), value.Expression, options);
    }

    private static ForAllElement ReadForAll(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var basePattern = reader.ReadProperty<PatternElement>(nameof(ForAllElement.BasePattern), options);
        if (basePattern is null)
        {
            throw new JsonException($"Failed to read {nameof(ForAllElement.BasePattern)} property value");
        }
        var patterns = reader.ReadArrayProperty<PatternElement>(nameof(ForAllElement.Patterns), options);
        return Element.ForAll(basePattern, patterns);
    }

    private static void WriteForAll(Utf8JsonWriter writer, JsonSerializerOptions options, ForAllElement value)
    {
        writer.WriteProperty(nameof(ForAllElement.BasePattern), value.BasePattern, options);
        writer.WriteArrayProperty(nameof(ForAllElement.Patterns), value.Patterns, options);
    }
}
