using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class InvokeExpressionSerializer : ExpressionSerializer<InvocationExpression>
{
    public InvokeExpressionSerializer()
        : base(ExpressionType.Invoke)
    {
    }

    public override InvocationExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var expression = reader.ReadProperty<Expression>(nameof(InvocationExpression.Expression), options);
        reader.TryReadArrayProperty<Expression>(nameof(InvocationExpression.Arguments), options, out var arguments);
        return Expression.Invoke(expression, arguments);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, InvocationExpression expression)
    {
        writer.WriteProperty(nameof(expression.Expression), expression.Expression, options);
        if (expression.Arguments.Any())
            writer.WriteArrayProperty(nameof(expression.Arguments), expression.Arguments, options);
    }
}
