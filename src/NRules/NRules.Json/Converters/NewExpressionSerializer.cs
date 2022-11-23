using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal sealed class NewExpressionSerializer : ExpressionSerializer<NewExpression>
{
    public NewExpressionSerializer()
        : base(ExpressionType.New)
    {
    }

    public override NewExpression InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var declaringType = reader.ReadProperty<Type>(nameof(NewExpression.Constructor.DeclaringType), options);
        reader.TryReadArrayProperty<Expression>(nameof(NewExpression.Arguments), options, out var arguments);

        var ctor = declaringType.GetConstructor(arguments.Select(x => x.Type).ToArray())
            ?? throw new ArgumentException($"Unable to find constructor. Type={declaringType}", nameof(declaringType));
        return Expression.New(ctor, arguments);
    }

    public override void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, NewExpression expression)
    {
        writer.WriteProperty(nameof(expression.Constructor.DeclaringType), expression.Constructor.DeclaringType, options);
        if (expression.Arguments.Any())
            writer.WriteArrayProperty(nameof(expression.Arguments), expression.Arguments, options);
    }
}
