using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Converters;

namespace NRules.Json
{
    public static class RuleSerializer
    {
        public static void Setup(JsonSerializerOptions options)
        {
            foreach (var converter in GetConverters())
            {
                options.Converters.Add(converter);
            }
        }

        public static IReadOnlyCollection<JsonConverter> GetConverters()
        {
            var converters = new List<JsonConverter>()
            {
                new TypeConverter(),
                new ExpressionConverter(),
                new RuleDefinitionConverter(),
                new RulePropertyConverter(),
                new NamedExpressionElementConverter(),
                new ActionElementConverter(),
                new RuleElementConverter(),
            };
            return converters;
        }
    }
}
