using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Converters;

namespace NRules.Json
{
    public static class JsonOptionsFactory
    {
        public static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            options.Converters.Add(new TypeConverter());
            options.Converters.Add(new ExpressionConverter());
            options.Converters.Add(new RuleDefinitionConverter());
            options.Converters.Add(new RulePropertyConverter());
            options.Converters.Add(new NamedExpressionElementConverter());
            options.Converters.Add(new ActionElementConverter());
            options.Converters.Add(new RuleElementConverter());
            return options;
        }
    }
}
