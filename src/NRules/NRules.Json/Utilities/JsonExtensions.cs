using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NRules.Json.Utilities
{
    internal static class JsonExtensions
    {
        public static void ReadStartObject(this ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();
            reader.Read();
        }

        public static void ReadStartArray(this ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();
            reader.Read();
        }

        public static void ReadPropertyName(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            var propertyName = reader.GetString();
            if (!name.NameEquals(propertyName, options))
                throw new JsonException();

            reader.Read();
        }

        public static bool TryReadPropertyName(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.EndObject ||
                reader.TokenType == JsonTokenType.EndArray)
                return false;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            var propertyName = reader.GetString();
            if (!name.NameEquals(propertyName, options))
                return false;

            reader.Read();
            return true;
        }

        public static string ReadStringProperty(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options)
        {
            reader.ReadPropertyName(name, options);
            
            string value = reader.GetString();
            reader.Read();
            
            return value;
        }

        public static bool TryReadStringProperty(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, out string value)
        {
            value = default;

            if (!reader.TryReadPropertyName(name, options))
                return false;

            value = reader.GetString();
            reader.Read();
            
            return true;
        }

        public static int ReadInt32Property(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options)
        {
            reader.ReadPropertyName(name, options);
            
            int value = reader.GetInt32();
            reader.Read();
            
            return value;
        }

        public static bool TryReadInt32Property(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, out int value)
        {
            value = default;
            if (!reader.TryReadPropertyName(name, options))
                return false;
            
            value = reader.GetInt32();
            reader.Read();
            
            return true;
        }

        public static TEnum ReadEnumProperty<TEnum>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options) where TEnum : struct
        {
            reader.ReadPropertyName(name, options);

            if (!Enum.TryParse(reader.GetString(), out TEnum value))
                throw new JsonException();
            reader.Read();
            
            return value;
        }

        public static bool TryReadEnumProperty<TEnum>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, out TEnum value) where TEnum : struct
        {
            value = default;
            if (!reader.TryReadPropertyName(name, options))
                return false;

            if (!Enum.TryParse(reader.GetString(), out value))
                throw new JsonException();
            reader.Read();
            
            return true;
        }

        public delegate TElement YieldElement<out TElement>(ref Utf8JsonReader reader, JsonSerializerOptions options);

        public static IReadOnlyCollection<TElement> ReadObjectArrayProperty<TElement>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, YieldElement<TElement> elementReader)
        {
            reader.ReadPropertyName(name, options);

            reader.ReadStartArray();

            var elements = new List<TElement>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();
                reader.Read();

                var element = elementReader(ref reader, options);
                elements.Add(element);

                if (reader.TokenType != JsonTokenType.EndObject)
                    throw new JsonException();
                reader.Read();
            }

            reader.Read();

            return elements;
        }

        public static bool TryReadObjectArrayProperty<TElement>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, YieldElement<TElement> elementReader, out IReadOnlyCollection<TElement> value)
        {
            value = Array.Empty<TElement>();

            if (!reader.TryReadPropertyName(name, options))
                return false;

            reader.ReadStartArray();

            var elements = new List<TElement>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();
                reader.Read();

                var element = elementReader(ref reader, options);
                elements.Add(element);

                if (reader.TokenType != JsonTokenType.EndObject)
                    throw new JsonException();
                reader.Read();
            }

            reader.Read();
            value = elements;
            return true;
        }

        public static IReadOnlyCollection<TElement> ReadArrayProperty<TElement>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options)
        {
            reader.ReadPropertyName(name, options);

            reader.ReadStartArray();

            var elements = new List<TElement>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var element = JsonSerializer.Deserialize<TElement>(ref reader, options);
                elements.Add(element);
                reader.Read();
            }

            reader.Read();

            return elements;
        }

        public static bool TryReadArrayProperty<TElement>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, out IReadOnlyCollection<TElement> value)
        {
            value = Array.Empty<TElement>();

            if (!reader.TryReadPropertyName(name, options))
                return false;

            reader.ReadStartArray();

            var elements = new List<TElement>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var element = JsonSerializer.Deserialize<TElement>(ref reader, options);
                elements.Add(element);
                reader.Read();
            }

            reader.Read();

            value = elements;
            return true;
        }

        public static bool TryReadArrayProperty(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, out string[] value)
        {
            value = Array.Empty<string>();

            if (!reader.TryReadPropertyName(name, options))
                return false;

            reader.ReadStartArray();

            var elements = new List<string>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var element = reader.GetString();
                elements.Add(element);
                reader.Read();
            }

            reader.Read();

            value = elements.ToArray();
            return true;
        }

        public static TElement ReadProperty<TElement>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options)
        {
            reader.ReadPropertyName(name, options);

            var value = JsonSerializer.Deserialize<TElement>(ref reader, options);
            reader.Read();
            return value;
        }

        public static object ReadProperty(this ref Utf8JsonReader reader, string name, Type type, JsonSerializerOptions options)
        {
            reader.ReadPropertyName(name, options);

            var value = JsonSerializer.Deserialize(ref reader, type, options);
            reader.Read();
            return value;
        }

        public static bool TryReadProperty<TElement>(this ref Utf8JsonReader reader, string name, JsonSerializerOptions options, out TElement value)
        {
            value = default;

            if (!reader.TryReadPropertyName(name, options))
                return false;

            value = JsonSerializer.Deserialize<TElement>(ref reader, options);
            reader.Read();
            return true;
        }

        public static void WriteStringProperty(this Utf8JsonWriter writer, string name, string value, JsonSerializerOptions options)
        {
            writer.WriteString(name.ToName(options), value);
        }

        public static void WriteNumberProperty(this Utf8JsonWriter writer, string name, int value, JsonSerializerOptions options)
        {
            writer.WriteNumber(name.ToName(options), value);
        }

        public static void WriteEnumProperty<TEnum>(this Utf8JsonWriter writer, string name, TEnum value, JsonSerializerOptions options) where TEnum: struct
        {
            writer.WriteString(name.ToName(options), value.ToString());
        }

        public static void WriteArrayProperty<TElement>(this Utf8JsonWriter writer, string name, IEnumerable<TElement> elements, JsonSerializerOptions options)
        {
            writer.WriteStartArray(name.ToName(options));
            foreach (var element in elements)
            {
                JsonSerializer.Serialize(writer, element, options);
            }
            writer.WriteEndArray();
        }

        public static void WriteArrayProperty(this Utf8JsonWriter writer, string name, IEnumerable<string> elements, JsonSerializerOptions options)
        {
            writer.WriteStartArray(name.ToName(options));
            foreach (var element in elements)
            {
                writer.WriteStringValue(element);
            }
            writer.WriteEndArray();
        }
        
        public static void WriteObjectArrayProperty<TElement>(this Utf8JsonWriter writer, string name, IEnumerable<TElement> elements, JsonSerializerOptions options, Action<TElement> elementAction)
        {
            writer.WriteStartArray(name.ToName(options));
            foreach (var element in elements)
            {
                writer.WriteStartObject();
                elementAction(element);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public static void WriteProperty<TValue>(this Utf8JsonWriter writer, string name, TValue value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(name.ToName(options));
            JsonSerializer.Serialize(writer, value, options);
        }

        public static void WriteProperty(this Utf8JsonWriter writer, string name, object value, Type valueType, JsonSerializerOptions options)
        {
            writer.WritePropertyName(name.ToName(options));
            JsonSerializer.Serialize(writer, value, valueType, options);
        }

        private static string ToName(this string name, JsonSerializerOptions options)
        {
            return options?.PropertyNamingPolicy?.ConvertName(name) ?? name;
        }

        private static bool NameEquals(this string rawExpectedName, string rawActualName, JsonSerializerOptions options)
        {
            var comparisonType = options?.PropertyNameCaseInsensitive ?? false
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture;
            return string.Equals(rawExpectedName.ToName(options), rawActualName.ToName(options), comparisonType);
        }
    }
}