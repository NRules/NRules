using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class ConstantExpressionSerializer : ExpressionSerializer<ConstantExpression>
{
    public ConstantExpressionSerializer()
        : base(ExpressionType.Constant)
    {
    }

    public override ConstantExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(ConstantExpression.Type), options);
        var value = reader.ReadProperty(nameof(ConstantExpression.Value), type, options);
        return Expression.Constant(value, type);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, ConstantExpression expression)
    {
        writer.WriteProperty(nameof(expression.Type), expression.Type, options);
        writer.WriteProperty(nameof(expression.Value), expression.Value, expression.Type, options);
    }
}
