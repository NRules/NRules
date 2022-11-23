using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class LambdaExpressionSerializer : ExpressionSerializer<LambdaExpression>
{
    private readonly ParameterExpressionSerializer _parameterSerializer;

    public LambdaExpressionSerializer(ParameterExpressionSerializer parameterSerializer)
        : base(ExpressionType.Lambda)
    {
        _parameterSerializer = parameterSerializer;
    }

    public override LambdaExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.TryReadProperty<Type>(nameof(LambdaExpression.Type), options, out var type);
        reader.TryReadObjectArrayProperty(nameof(LambdaExpression.Parameters), options, _parameterSerializer.InternalRead, out var parameters);
        var body = reader.ReadProperty<Expression>(nameof(LambdaExpression.Body), options);

        var expression = type is not null
            ? Expression.Lambda(type, body, parameters)
            : Expression.Lambda(body, parameters);

        var parameterCompactor = new ExpressionParameterCompactor();
        var result = parameterCompactor.Compact(expression);

        return result;
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, LambdaExpression expression)
    {
        var impliedDelegateType = expression.GetImpliedDelegateType();

        if (expression.Type != impliedDelegateType)
            writer.WriteProperty(nameof(expression.Type), expression.Type, options);

        if (expression.Parameters.Any())
            writer.WriteObjectArrayProperty(nameof(expression.Parameters), expression.Parameters, options, pe => _parameterSerializer.InternalWrite(writer, options, pe));

        writer.WriteProperty(nameof(expression.Body), expression.Body, options);
    }
}
