using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class UnaryExpressionSerializer : ExpressionSerializer<UnaryExpression>
{
    private readonly Func<Expression, Type, MethodInfo, UnaryExpression> _converter;

    public UnaryExpressionSerializer(ExpressionType type, Func<Expression, MethodInfo, UnaryExpression> converter)
        : this(type, (operand, _, method) => converter(operand, method))
    {
    }

    public UnaryExpressionSerializer(ExpressionType type, Func<Expression, Type, MethodInfo, UnaryExpression> converter)
        : base(type)
    {
        _converter = converter;
    }

    public override UnaryExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var operand = reader.ReadProperty<Expression>(nameof(UnaryExpression.Operand), options);
        reader.TryReadProperty<Type>(nameof(UnaryExpression.Type), options, out var type);

        var method = TryReadMethod(ref reader, options, operand);
        return _converter(operand, type, method);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, UnaryExpression expression)
    {
        writer.WriteProperty(nameof(UnaryExpression.Operand), expression.Operand, options);
        if (expression.Type != expression.Operand.Type)
            writer.WriteProperty(nameof(UnaryExpression.Type), expression.Type, options);
        if (expression.Method is not null && !expression.Method.IsSpecialName)
            writer.WriteMethodInfo(options, expression.Method, expression.Operand.Type);
    }
}
