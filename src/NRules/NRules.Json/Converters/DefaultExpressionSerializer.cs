using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class DefaultExpressionSerializer : ExpressionSerializer<DefaultExpression>
{
    public DefaultExpressionSerializer()
        : base(ExpressionType.Default)
    {
    }

    public override DefaultExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(DefaultExpression.Type), options);
        return Expression.Default(type);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, DefaultExpression expression)
    {
        writer.WriteProperty(nameof(expression.Type), expression.Type, options);
    }
}
