using System;
using System.Text.Json;

namespace NRules.Json
{
    internal static class JsonUtilities
    {
        public static string JsonName(string name, JsonSerializerOptions options)
        {
            return options.PropertyNamingPolicy?.ConvertName(name) ?? name;
        }

        public static bool JsonNameConvertEquals(string expected, string actual, JsonSerializerOptions options)
        {
            return JsonNameEquals(JsonName(expected, options), actual, options);
        }

        public static bool JsonNameEquals(string normalizedExpected, string actual, JsonSerializerOptions options)
        {
            var comparisonType = options.PropertyNameCaseInsensitive
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture;
            return string.Equals(normalizedExpected, JsonName(actual, options), comparisonType);
        }
    }
}
