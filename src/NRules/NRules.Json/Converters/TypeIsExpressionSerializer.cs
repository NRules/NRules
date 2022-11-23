using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class TypeIsExpressionSerializer : ExpressionSerializer<TypeBinaryExpression>
{
    public TypeIsExpressionSerializer()
        : base(ExpressionType.TypeIs)
    {
    }

    public override TypeBinaryExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var expression = reader.ReadProperty<Expression>(nameof(TypeBinaryExpression.Expression), options);
        var typeOperand = reader.ReadProperty<Type>(nameof(TypeBinaryExpression.TypeOperand), options);
        return Expression.TypeIs(expression, typeOperand!);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, TypeBinaryExpression expression)
    {
        writer.WriteProperty(nameof(expression.Expression), expression.Expression, options);
        writer.WriteProperty(nameof(expression.TypeOperand), expression.TypeOperand, options);
    }
}
