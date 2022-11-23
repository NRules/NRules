using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class MemberInitExpressionSerializer : ExpressionSerializer<MemberInitExpression>
{
    private readonly NewExpressionSerializer _newSerializer;

    public MemberInitExpressionSerializer(NewExpressionSerializer newSerializer)
        : base(ExpressionType.MemberInit)
    {
        _newSerializer = newSerializer;
    }

    public override MemberInitExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var newExpression = _newSerializer.InternalRead(ref reader, options);
        var bindings = reader.ReadObjectArrayProperty(nameof(MemberInitExpression.Bindings), options, ReadMemberBinding);
        return Expression.MemberInit(newExpression, bindings);

        MemberBinding ReadMemberBinding(ref Utf8JsonReader r, JsonSerializerOptions o)
        {
            return r.ReadMemberBinding(o, newExpression.Type);
        }
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, MemberInitExpression expression)
    {
        _newSerializer.InternalWrite(writer, options, expression.NewExpression);
        writer.WriteObjectArrayProperty(nameof(expression.Bindings), expression.Bindings, options, mb =>
        {
            writer.WriteMemberBinding(mb, options, expression.NewExpression.Type);
        });
    }
}
