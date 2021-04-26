using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using static NRules.Json.JsonUtilities;

namespace NRules.Json.Converters
{
    internal class ExpressionConverter : JsonConverter<Expression>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(Expression).IsAssignableFrom(typeToConvert);

        public override Expression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName && 
                !JsonNameConvertEquals(nameof(Expression.NodeType), reader.GetString(), options)) throw new JsonException();
            reader.Read();
            if (!Enum.TryParse(reader.GetString(), out ExpressionType nodeType)) throw new JsonException();

            Expression value;
            if (nodeType == ExpressionType.Lambda)
                value = ReadLambda(ref reader, options);
            else if (nodeType == ExpressionType.Parameter)
                value = ReadParameter(ref reader, options);
            else if (nodeType == ExpressionType.Constant)
                value = ReadConstant(ref reader, options);
            else if (nodeType == ExpressionType.MemberAccess)
                value = ReadMember(ref reader, options);
            else if (nodeType == ExpressionType.Call)
                value = ReadMethodCall(ref reader, options);
            else if (nodeType == ExpressionType.Equal ||
                     nodeType == ExpressionType.NotEqual ||
                     nodeType == ExpressionType.GreaterThanOrEqual ||
                     nodeType == ExpressionType.GreaterThan ||
                     nodeType == ExpressionType.LessThanOrEqual ||
                     nodeType == ExpressionType.LessThan ||
                     nodeType == ExpressionType.AndAlso ||
                     nodeType == ExpressionType.OrElse ||
                     nodeType == ExpressionType.And ||
                     nodeType == ExpressionType.Or ||
                     nodeType == ExpressionType.ExclusiveOr ||
                     nodeType == ExpressionType.Add ||
                     nodeType == ExpressionType.AddChecked ||
                     nodeType == ExpressionType.Divide ||
                     nodeType == ExpressionType.Modulo ||
                     nodeType == ExpressionType.Multiply ||
                     nodeType == ExpressionType.MultiplyChecked ||
                     nodeType == ExpressionType.Power ||
                     nodeType == ExpressionType.Subtract ||
                     nodeType == ExpressionType.SubtractChecked ||
                     nodeType == ExpressionType.Coalesce ||
                     nodeType == ExpressionType.ArrayIndex ||
                     nodeType == ExpressionType.LeftShift ||
                     nodeType == ExpressionType.RightShift)
                value = ReadBinaryExpression(ref reader, options, nodeType);
            else if (nodeType == ExpressionType.Not ||
                     nodeType == ExpressionType.Negate ||
                     nodeType == ExpressionType.NegateChecked ||
                     nodeType == ExpressionType.UnaryPlus ||
                     nodeType == ExpressionType.Convert ||
                     nodeType == ExpressionType.ConvertChecked ||
                     nodeType == ExpressionType.TypeAs)
                value = ReadUnaryExpression(ref reader, options, nodeType);
            else
                throw new NotSupportedException($"Unsupported expression type. NodeType={nodeType}");

            return value;
        }

        public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(JsonName(nameof(value.NodeType), options), value.NodeType.ToString());

            if (value is LambdaExpression le)
                WriteLambda(writer, options, le);
            else if (value is ParameterExpression pe)
                WriteParameter(writer, options, pe);
            else if (value is ConstantExpression ce)
                WriteConstant(writer, options, ce);
            else if (value is MemberExpression me)
                WriteMember(writer, options, me);
            else if (value is MethodCallExpression mce)
                WriteMethodCall(writer, options, mce);
            else if (value is BinaryExpression be)
                WriteBinaryExpression(writer, options, be);
            else if (value is UnaryExpression ue)
                WriteUnaryExpression(writer, options, ue);
            else
                throw new NotSupportedException($"Unsupported expression type. NodeType={value.NodeType}");

            writer.WriteEndObject();
        }

        private Expression ReadLambda(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Type type = default;
            Expression body = default;
            var parameters = new List<ParameterExpression>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(LambdaExpression.Type), options))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(LambdaExpression.Body), options))
                {
                    body = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(LambdaExpression.Parameters), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var parameter = JsonSerializer.Deserialize<ParameterExpression>(ref reader, options);
                        parameters.Add(parameter);
                    }
                }
            }

            var expression = Expression.Lambda(type!, body!, parameters);

            var parameterCompactor = new ExpressionParameterCompactor();
            var result = parameterCompactor.Compact(expression);

            return result;
        }

        private void WriteLambda(Utf8JsonWriter writer, JsonSerializerOptions options, LambdaExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(value.Type), options));
            JsonSerializer.Serialize(writer, value.Type, options);

            writer.WritePropertyName(JsonName(nameof(value.Body), options));
            JsonSerializer.Serialize(writer, value.Body, options);

            writer.WriteStartArray(JsonName(nameof(value.Parameters), options));
            foreach (var parameter in value.Parameters)
            {
                JsonSerializer.Serialize(writer, parameter, options);
            }
            writer.WriteEndArray();
        }

        private Expression ReadConstant(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName &&
                !JsonNameConvertEquals(nameof(ConstantExpression.Type), reader.GetString(), options)) throw new JsonException();
            
            reader.Read();
            var type = JsonSerializer.Deserialize<Type>(ref reader, options);

            reader.Read();
            var value = JsonSerializer.Deserialize(ref reader, type!, options);

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject) throw new JsonException();

            return Expression.Constant(value, type!);
        }

        private void WriteConstant(Utf8JsonWriter writer, JsonSerializerOptions options, ConstantExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(value.Type), options));
            JsonSerializer.Serialize(writer, value.Type, options);

            writer.WritePropertyName(JsonName(nameof(value.Value), options));
            JsonSerializer.Serialize(writer, value.Value, value.Type, options);
        }

        private Expression ReadParameter(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Type type = default;
            string name = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(ParameterExpression.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(ParameterExpression.Type), options))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            return Expression.Parameter(type!, name);
        }

        private void WriteParameter(Utf8JsonWriter writer, JsonSerializerOptions options, ParameterExpression value)
        {
            writer.WriteString(JsonName(nameof(value.Name), options), value.Name);
            
            writer.WritePropertyName(JsonName(nameof(value.Type), options));
            JsonSerializer.Serialize(writer, value.Type, options);
        }

        private Expression ReadMember(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
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

            return Expression.MakeMemberAccess(expression, member!);
        }

        private void WriteMember(Utf8JsonWriter writer, JsonSerializerOptions options, MemberExpression value)
        {
            writer.WriteString(JsonName(nameof(value.Member.MemberType), options), value.Member.MemberType.ToString());
            writer.WriteString(JsonName(nameof(value.Member.Name), options), value.Member.Name);
            writer.WritePropertyName(JsonName(nameof(value.Member.DeclaringType), options));
            JsonSerializer.Serialize(writer, value.Member.DeclaringType, options);

            writer.WritePropertyName(JsonName(nameof(value.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);
        }

        private Expression ReadMethodCall(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Expression @object = default;
            string name = default;
            Type declaringType = default;
            var arguments = new List<Expression>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(MethodCallExpression.Object), options))
                {
                    @object = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(MethodCallExpression.Method.Name), options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(MethodCallExpression.Method.DeclaringType), options))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(MethodCallExpression.Arguments), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var argument = JsonSerializer.Deserialize<Expression>(ref reader, options);
                        arguments.Add(argument);
                    }
                }
            }

            var method = declaringType!.GetMethod(name!, arguments.Select(x => x.Type).ToArray());

            return Expression.Call(@object, method!, arguments);
        }

        private void WriteMethodCall(Utf8JsonWriter writer, JsonSerializerOptions options, MethodCallExpression value)
        {
            writer.WriteString(JsonName(nameof(value.Method.Name), options), value.Method.Name);
            writer.WritePropertyName(JsonName(nameof(value.Method.DeclaringType), options));
            JsonSerializer.Serialize(writer, value.Method.DeclaringType, options);

            if (value.Object != null)
            {
                writer.WritePropertyName(JsonName(nameof(value.Object), options));
                JsonSerializer.Serialize(writer, value.Object, options);
            }

            if (value.Arguments.Any())
            {
                writer.WriteStartArray(JsonName(nameof(value.Arguments), options));
                foreach (var argument in value.Arguments)
                {
                    JsonSerializer.Serialize(writer, argument, options);
                }
                writer.WriteEndArray();
            }
        }

        private Expression ReadBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
        {
            Expression left = default;
            Expression right = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(BinaryExpression.Left), options))
                {
                    left = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(BinaryExpression.Right), options))
                {
                    right = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
            }

            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return Expression.Equal(left!, right!);
                case ExpressionType.NotEqual:
                    return Expression.NotEqual(left!, right!);
                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left!, right!);
                case ExpressionType.LessThan:
                    return Expression.LessThan(left!, right!);
                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left!, right!);
                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(left!, right!);
                case ExpressionType.AndAlso:
                    return Expression.AndAlso(left!, right!);
                case ExpressionType.OrElse:
                    return Expression.OrElse(left!, right!);
                case ExpressionType.And:
                    return Expression.And(left!, right!);
                case ExpressionType.Or:
                    return Expression.Or(left!, right!);
                case ExpressionType.ExclusiveOr:
                    return Expression.ExclusiveOr(left!, right!);
                case ExpressionType.Add:
                    return Expression.Add(left!, right!);
                case ExpressionType.AddChecked:
                    return Expression.AddChecked(left!, right!);
                case ExpressionType.Divide:
                    return Expression.Divide(left!, right!);
                case ExpressionType.Modulo:
                    return Expression.Modulo(left!, right!);
                case ExpressionType.Multiply:
                    return Expression.Multiply(left!, right!);
                case ExpressionType.MultiplyChecked:
                    return Expression.MultiplyChecked(left!, right!);
                case ExpressionType.Power:
                    return Expression.Power(left!, right!);
                case ExpressionType.Subtract:
                    return Expression.Subtract(left!, right!);
                case ExpressionType.SubtractChecked:
                    return Expression.SubtractChecked(left!, right!);
                case ExpressionType.Coalesce:
                    return Expression.Coalesce(left!, right!);
                case ExpressionType.ArrayIndex:
                    return Expression.ArrayIndex(left!, right!);
                case ExpressionType.LeftShift:
                    return Expression.LeftShift(left!, right!);
                case ExpressionType.RightShift:
                    return Expression.RightShift(left!, right!);
                default:
                    throw new NotSupportedException($"Unrecognized binary expression: {expressionType}");
            }
        }

        private void WriteBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BinaryExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(BinaryExpression.Left), options));
            JsonSerializer.Serialize(writer, value.Left, options);
            
            writer.WritePropertyName(JsonName(nameof(BinaryExpression.Right), options));
            JsonSerializer.Serialize(writer, value.Right, options);
        }

        private Expression ReadUnaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
        {
            Expression operand = default;
            Type type = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(UnaryExpression.Operand), options))
                {
                    operand = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(UnaryExpression.Type), options))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            switch (expressionType)
            {
                case ExpressionType.Not:
                    return Expression.Not(operand!);
                case ExpressionType.Negate:
                    return Expression.Negate(operand!);
                case ExpressionType.NegateChecked:
                    return Expression.NegateChecked(operand!);
                case ExpressionType.UnaryPlus:
                    return Expression.UnaryPlus(operand!);
                case ExpressionType.Convert:
                    return Expression.Convert(operand!, type!);
                case ExpressionType.ConvertChecked:
                    return Expression.ConvertChecked(operand!, type!);
                case ExpressionType.TypeAs:
                    return Expression.TypeAs(operand!, type!);
                default:
                    throw new NotSupportedException($"Unrecognized unary expression: {expressionType}");
            }
        }

        private void WriteUnaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, UnaryExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(UnaryExpression.Operand), options));
            JsonSerializer.Serialize(writer, value.Operand, options);

            if (value.Type != value.Operand.Type)
            {
                writer.WritePropertyName(JsonName(nameof(UnaryExpression.Type), options));
                JsonSerializer.Serialize(writer, value.Type, options);
            }
        }
    }
}
