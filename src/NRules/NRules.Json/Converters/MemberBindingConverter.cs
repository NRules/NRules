using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using static NRules.Json.JsonUtilities;

namespace NRules.Json.Converters
{
    internal class MemberBindingConverter : JsonConverter<MemberBinding>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(MemberBinding).IsAssignableFrom(typeToConvert);

        public override MemberBinding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName && 
                !JsonNameConvertEquals(nameof(MemberBinding.BindingType), reader.GetString(), options)) throw new JsonException();
            reader.Read();
            if (!Enum.TryParse(reader.GetString(), out MemberBindingType bindingType)) throw new JsonException();

            MemberBinding value;
            if (bindingType == MemberBindingType.Assignment)
                value = ReadMemberAssignment(ref reader, options);
            else
                throw new NotSupportedException($"Unsupported member binding type. BindingType={bindingType}");

            return value;

        }

        public override void Write(Utf8JsonWriter writer, MemberBinding value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(JsonName(nameof(value.BindingType), options), value.BindingType.ToString());

            if (value is MemberAssignment ma)
                WriteMemberAssignment(writer, options, ma);
            else
                throw new NotSupportedException($"Unsupported member binding type. BindingType={value.BindingType}");

            writer.WriteEndObject();
        }

        private MemberAssignment ReadMemberAssignment(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            //TODO: Consolidate with ExpressionConverter.ReadMemberExpression
            Expression expression = default;
            MemberTypes memberType = default;
            string name = default;
            Type declaringType = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(MemberExpression.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(MemberExpression.Member.MemberType), options))
                {
                    Enum.TryParse(reader.GetString(), out memberType);
                }
                else if (JsonNameEquals(propertyName, nameof(MemberExpression.Member.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(MemberExpression.Member.DeclaringType), options))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            MemberInfo member;
            if (memberType == MemberTypes.Field)
                member = declaringType!.GetField(name!);
            else if (memberType == MemberTypes.Property)
                member = declaringType!.GetProperty(name!);
            else
                throw new NotSupportedException($"MemberType={memberType}");

            return Expression.Bind(member!, expression!);
        }

        private void WriteMemberAssignment(Utf8JsonWriter writer, JsonSerializerOptions options, MemberAssignment value)
        {
            //TODO: Consolidate with ExpressionConverter.WriteMemberExpression
            writer.WriteString(JsonName(nameof(value.Member.MemberType), options), value.Member.MemberType.ToString());
            writer.WriteString(JsonName(nameof(value.Member.Name), options), value.Member.Name);
            writer.WritePropertyName(JsonName(nameof(value.Member.DeclaringType), options));
            JsonSerializer.Serialize(writer, value.Member.DeclaringType, options);

            writer.WritePropertyName(JsonName(nameof(value.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);
        }
    }
}
