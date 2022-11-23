using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class CallExpressionSerializer : ExpressionSerializer<MethodCallExpression>
{
    public CallExpressionSerializer()
        : base(ExpressionType.Call)
    {
    }

    public override MethodCallExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.TryReadProperty<Expression>(nameof(MethodCallExpression.Object), options, out var @object);
        var methodRecord = reader.ReadMethodInfo(options);
        reader.TryReadArrayProperty<Expression>(nameof(MethodCallExpression.Arguments), options, out var arguments);
        var method = methodRecord.GetMethod(arguments.Select(x => x.Type).ToArray(), @object?.Type);
        return Expression.Call(@object, method, arguments);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, MethodCallExpression expression)
    {
        if (expression.Object is not null)
            writer.WriteProperty(nameof(expression.Object), expression.Object, options);
        writer.WriteMethodInfo(options, expression.Method, expression.Object?.Type);
        if (expression.Arguments.Any())
            writer.WriteArrayProperty(nameof(expression.Arguments), expression.Arguments, options);
    }
}
