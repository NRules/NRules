using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class BlockExpressionSerializer : ExpressionSerializer<BlockExpression>
{
    private readonly ParameterExpressionSerializer _parameterSerializer;

    public BlockExpressionSerializer(ParameterExpressionSerializer parameterSerializer)
        : base(ExpressionType.Block)
    {
        _parameterSerializer = parameterSerializer;
    }

    public override BlockExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(BlockExpression.Type), options);
        if (type is null)
            throw new JsonException($"Failed to read {nameof(BlockExpression.Type)} property value");

        reader.TryReadObjectArrayProperty(nameof(BlockExpression.Variables), options, _parameterSerializer.InternalRead, out var variables);
        var expressions = reader.ReadArrayProperty<Expression>(nameof(BlockExpression.Expressions), options);
        return Expression.Block(type, variables, expressions);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, BlockExpression expression)
    {
        writer.WriteProperty(nameof(expression.Type), expression.Type, options);
        writer.WriteObjectArrayProperty(nameof(expression.Variables), expression.Variables, options, variable => _parameterSerializer.InternalWrite(writer, options, variable));
        writer.WriteArrayProperty(nameof(expression.Expressions), expression.Expressions, options);
    }
}
