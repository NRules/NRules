using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                reader.GetString() != nameof(Expression.NodeType)) throw new JsonException();
            reader.Read();
            if (!Enum.TryParse(reader.GetString(), out ExpressionType nodeType)) throw new JsonException();

            Expression value;
            if (nodeType == ExpressionType.Lambda)
                value = ReadLambda(ref reader, options);
            else if (nodeType == ExpressionType.Parameter)
                value = ReadParameter(ref reader, options);
            else if (nodeType == ExpressionType.MemberAccess)
                value = ReadMember(ref reader, options);
            else if (nodeType == ExpressionType.Call)
                value = ReadMethodCall(ref reader, options);
            else
                throw new NotSupportedException($"Unsupported expression type. NodeType={nodeType}");

            return value;
        }
        
        public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(value.NodeType), value.NodeType.ToString());

            if (value is LambdaExpression le)
                WriteLambda(writer, options, le);
            else if (value is ParameterExpression pe)
                WriteParameter(writer, options, pe);
            else if (value is MemberExpression me)
                WriteMember(writer, options, me);
            else if (value is MethodCallExpression mce)
                WriteMethodCall(writer, options, mce);
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
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(LambdaExpression.Type))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (propertyName == nameof(LambdaExpression.Body))
                {
                    body = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (propertyName == nameof(LambdaExpression.Parameters))
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
            writer.WritePropertyName(nameof(value.Type));
            JsonSerializer.Serialize(writer, value.Type, options);

            writer.WritePropertyName(nameof(value.Body));
            JsonSerializer.Serialize(writer, value.Body, options);

            writer.WriteStartArray(nameof(value.Parameters));
            foreach (var parameter in value.Parameters)
            {
                JsonSerializer.Serialize(writer, parameter, options);
            }
            writer.WriteEndArray();
        }

        private Expression ReadParameter(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Type type = default;
            string name = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(ParameterExpression.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(ParameterExpression.Type))
                {
                    type = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            return Expression.Parameter(type!, name);
        }

        private void WriteParameter(Utf8JsonWriter writer, JsonSerializerOptions options, ParameterExpression value)
        {
            writer.WriteString(nameof(value.Name), value.Name);
            
            writer.WritePropertyName(nameof(value.Type));
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
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(MemberExpression.Expression))
                {
                    expression = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (propertyName == nameof(MemberExpression.Member.MemberType))
                {
                    Enum.TryParse(reader.GetString(), out memberType);
                }
                else if (propertyName == nameof(MemberExpression.Member.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(MemberExpression.Member.DeclaringType))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            MemberInfo member;
            if (memberType == MemberTypes.Field)
                member = declaringType.GetField(name);
            else if (memberType == MemberTypes.Property)
                member = declaringType.GetProperty(name);
            else
                throw new NotSupportedException($"MemberType={memberType}");

            return Expression.MakeMemberAccess(expression, member!);
        }

        private void WriteMember(Utf8JsonWriter writer, JsonSerializerOptions options, MemberExpression value)
        {
            writer.WriteString(nameof(value.Member.MemberType), value.Member.MemberType.ToString());
            writer.WriteString(nameof(value.Member.Name), value.Member.Name);
            writer.WritePropertyName(nameof(value.Member.DeclaringType));
            JsonSerializer.Serialize(writer, value.Member.DeclaringType, options);

            writer.WritePropertyName(nameof(value.Expression));
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
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == nameof(MethodCallExpression.Object))
                {
                    @object = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (propertyName == nameof(MethodCallExpression.Method.Name))
                {
                    name = reader.GetString();
                }
                else if (propertyName == nameof(MethodCallExpression.Method.DeclaringType))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (propertyName == nameof(MethodCallExpression.Arguments))
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
            writer.WriteString(nameof(value.Method.Name), value.Method.Name);
            writer.WritePropertyName(nameof(value.Method.DeclaringType));
            JsonSerializer.Serialize(writer, value.Method.DeclaringType, options);

            if (value.Object != null)
            {
                writer.WritePropertyName(nameof(value.Object));
                JsonSerializer.Serialize(writer, value.Object, options);
            }

            writer.WriteStartArray(nameof(value.Arguments));
            foreach (var argument in value.Arguments)
            {
                JsonSerializer.Serialize(writer, argument, options);
            }
            writer.WriteEndArray();
        }
    }
}
