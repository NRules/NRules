using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class ListInitExpressionSerializer : ExpressionSerializer<ListInitExpression>
{
    private readonly NewExpressionSerializer _newSerializer;

    public ListInitExpressionSerializer(NewExpressionSerializer newSerializer)
        : base(ExpressionType.ListInit)
    {
        _newSerializer = newSerializer;
    }

    public override ListInitExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var newExpression = _newSerializer.InternalRead(ref reader, options);
        var initializers = reader.ReadObjectArrayProperty(nameof(ListInitExpression.Initializers), options, ReadElementInit);
        return Expression.ListInit(newExpression!, initializers);

        ElementInit ReadElementInit(ref Utf8JsonReader r, JsonSerializerOptions o)
        {
            var methodRecord = r.ReadMethodInfo(o);
            var arguments = r.ReadArrayProperty<Expression>(nameof(ElementInit.Arguments), o);
            var method = methodRecord.GetMethod(arguments.Select(x => x.Type).ToArray(), newExpression.Type);
            return Expression.ElementInit(method, arguments);
        }
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, ListInitExpression expression)
    {
        _newSerializer.InternalWrite(writer, options, expression.NewExpression);

        writer.WriteObjectArrayProperty(nameof(expression.Initializers), expression.Initializers, options, initializer =>
        {
            writer.WriteMethodInfo(options, initializer.AddMethod, expression.Type);
            writer.WriteArrayProperty(nameof(initializer.Arguments), initializer.Arguments, options);
        });
    }
}
