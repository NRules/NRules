using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace NRules.Json.Converters;

internal abstract class ExpressionSerializer<T> : IExpressionSerializer
    where T : Expression
{
    protected ExpressionSerializer(ExpressionType supportedType)
    {
        SupportedType = supportedType;
    }

    public ExpressionType SupportedType { get; }

    public Expression Read(ref Utf8JsonReader reader, JsonSerializerOptions options) => InternalRead(ref reader, options);

    public void Write(Utf8JsonWriter writer, JsonSerializerOptions options, Expression expression) => InternalWrite(writer, options, (T)expression);

    public abstract T InternalRead(ref Utf8JsonReader reader, JsonSerializerOptions options);

    public abstract void InternalWrite(Utf8JsonWriter writer, JsonSerializerOptions options, T expression);

    protected MethodInfo TryReadMethod(ref Utf8JsonReader reader, JsonSerializerOptions options, Expression operand)
    {
        if (!reader.TryReadMethodInfo(options, out var methodRecord))
            return default;

        var argumentTypes = new[] { operand.Type };
        return methodRecord.GetMethod(argumentTypes, argumentTypes[0]);
    }

    protected MethodInfo TryReadMethod(ref Utf8JsonReader reader, JsonSerializerOptions options, Expression left, Expression right)
    {
        if (!reader.TryReadMethodInfo(options, out var methodRecord))
            return default;

        var argumentTypes = new[] { left.Type, right.Type };
        return methodRecord.GetMethod(argumentTypes, argumentTypes[0]);
    }
}
