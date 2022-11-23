using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class ConditionalExpressionSerializer : ExpressionSerializer<ConditionalExpression>
{
    public ConditionalExpressionSerializer()
        : base(ExpressionType.Conditional)
    {
    }

    public override ConditionalExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var test = reader.ReadProperty<Expression>(nameof(ConditionalExpression.Test), options);
        var ifTrue = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfTrue), options);
        var ifFalse = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfFalse), options);
        return Expression.Condition(test, ifTrue, ifFalse);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, ConditionalExpression expression)
    {
        writer.WriteProperty(nameof(expression.Test), expression.Test, options);
        writer.WriteProperty(nameof(expression.IfTrue), expression.IfTrue, options);
        writer.WriteProperty(nameof(expression.IfFalse), expression.IfFalse, options);
    }
}
