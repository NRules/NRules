using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class BinaryExpressionSerializer : ExpressionSerializer<BinaryExpression>
{
    private readonly Func<Expression, Expression, MethodInfo, BinaryExpression> _converter;

    public BinaryExpressionSerializer(ExpressionType type, Func<Expression, Expression, bool, MethodInfo, BinaryExpression> converter, bool liftToNull)
        : this(type, (left, right, method) => converter(left, right, liftToNull, method))
    {
    }

    public BinaryExpressionSerializer(ExpressionType type, Func<Expression, Expression, MethodInfo, BinaryExpression> converter)
        : base(type)
    {
        _converter = converter;
    }

    public override BinaryExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var left = reader.ReadProperty<Expression>(nameof(BinaryExpression.Left), options);
        var right = reader.ReadProperty<Expression>(nameof(BinaryExpression.Right), options);
        var method = TryReadMethod(ref reader, options, left, right);
        return _converter(left, right, method);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, BinaryExpression expression)
    {
        writer.WriteProperty(nameof(BinaryExpression.Left), expression.Left, options);
        writer.WriteProperty(nameof(BinaryExpression.Right), expression.Right, options);

        if (expression.Method is not null && !expression.Method.IsSpecialName)
            writer.WriteMethodInfo(options, expression.Method, expression.Left.Type);
    }
}
