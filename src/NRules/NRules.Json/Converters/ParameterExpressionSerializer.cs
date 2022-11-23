using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class ParameterExpressionSerializer : ExpressionSerializer<ParameterExpression>
{
    public ParameterExpressionSerializer()
        : base(ExpressionType.Parameter)
    {
    }

    public override ParameterExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty(nameof(ParameterExpression.Name), options);
        var type = reader.ReadProperty<Type>(nameof(ParameterExpression.Type), options);
        return Expression.Parameter(type, name);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, ParameterExpression expression)
    {
        writer.WriteStringProperty(nameof(expression.Name), expression.Name, options);
        writer.WriteProperty(nameof(expression.Type), expression.Type, options);
    }
}
