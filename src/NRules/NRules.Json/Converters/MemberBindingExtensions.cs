using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Utilities;

namespace NRules.Json.Converters
{
    internal static class MemberBindingExtensions
    {
        public static MemberBinding ReadMemberBinding(this ref Utf8JsonReader reader, JsonSerializerOptions options, Type impiedType = null)
        {
            var bindingType = reader.ReadEnumProperty<MemberBindingType>(nameof(MemberBinding.BindingType), options);

            switch (bindingType)
            {
                case MemberBindingType.Assignment:
                    return ReadMemberAssignment(ref reader, options, impiedType);
                default:
                    throw new NotSupportedException($"Unsupported member binding type. BindingType={bindingType}");
            }
        }

        public static void WriteMemberBinding(this Utf8JsonWriter writer, MemberBinding value, JsonSerializerOptions options, Type impliedType = null)
        {
            writer.WriteEnumProperty(nameof(value.BindingType), value.BindingType, options);

            switch (value)
            {
                case MemberAssignment ma:
                    WriteMemberAssignment(writer, options, ma, impliedType);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported member binding type. BindingType={value.BindingType}");
            }
        }

        private static MemberAssignment ReadMemberAssignment(ref Utf8JsonReader reader, JsonSerializerOptions options, Type impliedType)
        {
            var member = reader.ReadMemberInfo(options, impliedType);
            var expression = reader.ReadProperty<Expression>(nameof(MemberExpression.Expression), options);
            return Expression.Bind(member, expression);
        }

        private static void WriteMemberAssignment(Utf8JsonWriter writer, JsonSerializerOptions options, MemberAssignment value, Type impliedType)
        {
            writer.WriteMemberInfo(options, value.Member, impliedType);
            writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        }
    }
}
