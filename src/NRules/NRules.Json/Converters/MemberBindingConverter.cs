using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;

namespace NRules.Json.Converters
{
    internal class MemberBindingConverter : JsonConverter<MemberBinding>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(MemberBinding).IsAssignableFrom(typeToConvert);

        public override MemberBinding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ReadStartObject();
            var bindingType = reader.ReadEnumProperty<MemberBindingType>(nameof(MemberBinding.BindingType), options);

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
            writer.WriteEnumProperty(nameof(value.BindingType), value.BindingType, options);

            if (value is MemberAssignment ma)
                WriteMemberAssignment(writer, options, ma);
            else
                throw new NotSupportedException($"Unsupported member binding type. BindingType={value.BindingType}");

            writer.WriteEndObject();
        }

        private MemberAssignment ReadMemberAssignment(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var member = reader.ReadMemberInfo(options);
            var expression = reader.ReadProperty<Expression>(nameof(MemberExpression.Expression), options);
            return Expression.Bind(member, expression);
        }

        private void WriteMemberAssignment(Utf8JsonWriter writer, JsonSerializerOptions options, MemberAssignment value)
        {
            writer.WriteMemberInfo(options, value.Member);
            writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        }
    }
}
