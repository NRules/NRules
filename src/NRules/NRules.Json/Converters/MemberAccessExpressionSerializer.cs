using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class MemberAccessExpressionSerializer : ExpressionSerializer<MemberExpression>
{
    public MemberAccessExpressionSerializer()
        : base(ExpressionType.MemberAccess)
    {
    }

    public override MemberExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var member = reader.ReadMemberInfo(options);
        reader.TryReadProperty<Expression>(nameof(MemberExpression.Expression), options, out var expression);
        return Expression.MakeMemberAccess(expression, member);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, MemberExpression expression)
    {
        writer.WriteMemberInfo(options, expression.Member);
        if (expression.Expression is not null)
            writer.WriteProperty(nameof(expression.Expression), expression.Expression, options);
    }
}
