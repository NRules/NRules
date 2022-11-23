using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class NewArrayInitExpressionSerializer : ExpressionSerializer<NewArrayExpression>
{
    public NewArrayInitExpressionSerializer()
        : base(ExpressionType.NewArrayInit)
    {
    }

    public override NewArrayExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var elementType = reader.ReadProperty<Type>("ElementType", options);
        reader.TryReadArrayProperty<Expression>(nameof(NewArrayExpression.Expressions), options, out var expressions);
        return Expression.NewArrayInit(elementType, expressions);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, NewArrayExpression expression)
    {
        writer.WriteProperty("ElementType", expression.Type.GetElementType(), options);
        if (expression.Expressions.Any())
            writer.WriteArrayProperty(nameof(expression.Expressions), expression.Expressions, options);
    }
}
