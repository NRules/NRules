using System.Linq.Expressions;
using System.Text.Json;

namespace NRules.Json.Converters;

internal interface IExpressionSerializer
{
    ExpressionType SupportedType { get; }

    Expression Read(ref Utf8JsonReader reader, JsonSerializerOptions options);

    void Write(Utf8JsonWriter writer, JsonSerializerOptions options, Expression expression);
}
