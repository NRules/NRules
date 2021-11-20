using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Converters;
using NRules.RuleModel;

namespace NRules.Json
{
    /// <summary>
    /// Provides functionality to serialize rules to JSON and deserialize JSON
    /// into rules.
    /// </summary>
    public static class RuleSerializer
    {
        /// <summary>
        /// Configures <see cref="JsonSerializerOptions"/>, so that it can be used with the
        /// <see cref="JsonSerializer"/> to serialize <see cref="IRuleDefinition"/> objects to JSON
        /// and deserialize JSON into <see cref="IRuleDefinition"/> objects.
        /// </summary>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to configure for rules serialization.</param>
        public static void Setup(JsonSerializerOptions options)
        {
            Setup(options, GetDefaultTypeResolver());
        }

        /// <summary>
        /// Configures <see cref="JsonSerializerOptions"/>, so that it can be used with the
        /// <see cref="JsonSerializer"/> to serialize <see cref="IRuleDefinition"/> objects to JSON
        /// and deserialize JSON into <see cref="IRuleDefinition"/> objects.
        /// </summary>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to configure for rules serialization.</param>
        /// <param name="typeResolver">Type resolver that converts CLR types to type names and type names to CLR types.</param>
        public static void Setup(JsonSerializerOptions options, ITypeResolver typeResolver)
        {
            foreach (var converter in GetConverters(typeResolver))
            {
                options.Converters.Add(converter);
            }
        }

        /// <summary>
        /// Creates custom JSON converters that can be used with <see cref="JsonSerializerOptions"/> and
        /// <see cref="JsonSerializer"/> to serialize <see cref="IRuleDefinition"/> objects to JSON
        /// and deserialize JSON into <see cref="IRuleDefinition"/> objects.
        /// </summary>
        /// <returns>Collection of JSON converters necessary for rules serialization.</returns>
        public static IReadOnlyCollection<JsonConverter> GetConverters()
        {
            return GetConverters(GetDefaultTypeResolver());
        }

        /// <summary>
        /// Creates custom JSON converters that can be used with <see cref="JsonSerializerOptions"/> and
        /// <see cref="JsonSerializer"/> to serialize <see cref="IRuleDefinition"/> objects to JSON
        /// and deserialize JSON into <see cref="IRuleDefinition"/> objects.
        /// </summary>
        /// <param name="typeResolver">Type resolver that converts CLR types to type names and type names to CLR types.</param>
        /// <returns>Collection of JSON converters necessary for rules serialization.</returns>
        public static IReadOnlyCollection<JsonConverter> GetConverters(ITypeResolver typeResolver)
        {
            var converters = new List<JsonConverter>()
            {
                new TypeConverter(typeResolver),
                new ExpressionConverter(),
                new MemberBindingConverter(),
                new RuleDefinitionConverter(),
                new RulePropertyConverter(),
                new NamedExpressionElementConverter(),
                new ActionElementConverter(),
                new RuleElementConverter(),
            };
            return converters;
        }

        private static TypeResolver GetDefaultTypeResolver()
        {
            var typeResolver = new TypeResolver();
            typeResolver.RegisterDefaultAliases();
            return typeResolver;
        }
    }
}
