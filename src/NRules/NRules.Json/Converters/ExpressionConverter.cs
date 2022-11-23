using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal class ExpressionConverter : JsonConverter<Expression>
{
    private readonly IExpressionSerializerCollection _serializers;

    public ExpressionConverter()
        : this(new ExpressionSerializerCollection())
    {
    }

    public ExpressionConverter(IExpressionSerializerCollection serializers)
    {
        _serializers = serializers;
    }

    public override bool CanConvert(Type typeToConvert) =>
        typeof(Expression).IsAssignableFrom(typeToConvert);

    public override Expression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();
        var nodeType = reader.ReadEnumProperty<ExpressionType>(nameof(Expression.NodeType), options);

        return _serializers.GetSerializer(nodeType).Read(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteEnumProperty(nameof(value.NodeType), value.NodeType, options);

        _serializers.GetSerializer(value.NodeType).Write(writer, options, value);

        writer.WriteEndObject();
    }
}
