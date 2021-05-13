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
            else if (nodeType == ExpressionType.Invoke)
                value = ReadInvocationExpression(ref reader, options);
            else if (nodeType == ExpressionType.TypeIs)
                value = ReadTypeBinaryExpressions(ref reader, options);
            else if (nodeType == ExpressionType.New)
                value = ReadNewExpressions(ref reader, options);
            else if (nodeType == ExpressionType.NewArrayInit)
                value = ReadNewArrayInitExpressions(ref reader, options);
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
            else if (value is InvocationExpression ie)
                WriteInvocationExpression(writer, options, ie);
            else if (value is TypeBinaryExpression tbe)
                WriteTypeBinaryExpressions(writer, options, tbe);
            else if (value is NewExpression ne)
                WriteNewExpressions(writer, options, ne);
            else if (value is NewArrayExpression nae)
                WriteNewArrayInitExpressions(writer, options, nae);
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

            var expression = type != null
                ? Expression.Lambda(type, body!, parameters)
                : Expression.Lambda(body!, parameters);

            var parameterCompactor = new ExpressionParameterCompactor();
            var result = parameterCompactor.Compact(expression);

            return result;
        }

        private void WriteLambda(Utf8JsonWriter writer, JsonSerializerOptions options, LambdaExpression value)
        {
            var impliedDelegateType = GetImpliedDelegateType(value);
            if (value.Type != impliedDelegateType)
            {
                writer.WritePropertyName(JsonName(nameof(value.Type), options));
                JsonSerializer.Serialize(writer, value.Type, options);
            }

            writer.WritePropertyName(JsonName(nameof(value.Body), options));
            JsonSerializer.Serialize(writer, value.Body, options);

            if (value.Parameters.Any())
            {
                writer.WriteStartArray(JsonName(nameof(value.Parameters), options));
                foreach (var parameter in value.Parameters)
                {
                    JsonSerializer.Serialize(writer, parameter, options);
                }
                writer.WriteEndArray();
            }
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

            if (value.Expression != null)
            {
                writer.WritePropertyName(JsonName(nameof(value.Expression), options));
                JsonSerializer.Serialize(writer, value.Expression, options);
            }
        }

        private Expression ReadMethodCall(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Expression @object = default;
            var arguments = new List<Expression>();
            string name = default;
            Type declaringType = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(MethodCallExpression.Object), options))
                {
                    @object = JsonSerializer.Deserialize<Expression>(ref reader, options);
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
                else if (JsonNameEquals(propertyName, "MethodName", options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(MethodInfo.DeclaringType), options))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            var typeForMethod = declaringType ?? @object!.Type;
            var method = typeForMethod.GetMethod(name!, arguments.Select(a => a.Type).ToArray());
            return Expression.Call(@object, method!, arguments);
        }

        private void WriteMethodCall(Utf8JsonWriter writer, JsonSerializerOptions options, MethodCallExpression value)
        {
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

            WriteMethodInfo(writer, options, value.Method, value.Object?.Type);
        }

        private Expression ReadBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
        {
            Expression left = default;
            Expression right = default;
            string name = default;
            Type declaringType = default;

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
                else if (JsonNameEquals(propertyName, "MethodName", options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(MethodInfo.DeclaringType), options))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            MethodInfo method = default;
            if (name != null)
            {
                var typeForMethod = declaringType ?? left!.Type;
                method = typeForMethod.GetMethod(name, new[] {left!.Type, right!.Type});
            }

            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return Expression.Equal(left!, right!, false, method);
                case ExpressionType.NotEqual:
                    return Expression.NotEqual(left!, right!, false, method);
                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left!, right!, false, method);
                case ExpressionType.LessThan:
                    return Expression.LessThan(left!, right!, false, method);
                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left!, right!, false, method);
                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(left!, right!, false, method);
                case ExpressionType.AndAlso:
                    return Expression.AndAlso(left!, right!, method);
                case ExpressionType.OrElse:
                    return Expression.OrElse(left!, right!, method);
                case ExpressionType.And:
                    return Expression.And(left!, right!, method);
                case ExpressionType.Or:
                    return Expression.Or(left!, right!, method);
                case ExpressionType.ExclusiveOr:
                    return Expression.ExclusiveOr(left!, right!, method);
                case ExpressionType.Add:
                    return Expression.Add(left!, right!, method);
                case ExpressionType.AddChecked:
                    return Expression.AddChecked(left!, right!, method);
                case ExpressionType.Divide:
                    return Expression.Divide(left!, right!, method);
                case ExpressionType.Modulo:
                    return Expression.Modulo(left!, right!, method);
                case ExpressionType.Multiply:
                    return Expression.Multiply(left!, right!, method);
                case ExpressionType.MultiplyChecked:
                    return Expression.MultiplyChecked(left!, right!, method);
                case ExpressionType.Power:
                    return Expression.Power(left!, right!, method);
                case ExpressionType.Subtract:
                    return Expression.Subtract(left!, right!, method);
                case ExpressionType.SubtractChecked:
                    return Expression.SubtractChecked(left!, right!, method);
                case ExpressionType.Coalesce:
                    return Expression.Coalesce(left!, right!);
                case ExpressionType.ArrayIndex:
                    return Expression.ArrayIndex(left!, right!);
                case ExpressionType.LeftShift:
                    return Expression.LeftShift(left!, right!, method);
                case ExpressionType.RightShift:
                    return Expression.RightShift(left!, right!, method);
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

            if (value.Method != null && !value.Method.IsSpecialName)
            {
                WriteMethodInfo(writer, options, value.Method, value.Left.Type);
            }
        }

        private Expression ReadUnaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
        {
            Expression operand = default;
            Type type = default;
            string name = default;
            Type declaringType = default;

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
                else if (JsonNameEquals(propertyName, "MethodName", options))
                {
                    name = reader.GetString();
                }
                else if (JsonNameEquals(propertyName, nameof(MethodInfo.DeclaringType), options))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            MethodInfo method = default;
            if (name != null)
            {
                var typeForMethod = declaringType ?? operand!.Type;
                method = typeForMethod.GetMethod(name, new[] {operand!.Type});
            }

            switch (expressionType)
            {
                case ExpressionType.Not:
                    return Expression.Not(operand!, method);
                case ExpressionType.Negate:
                    return Expression.Negate(operand!, method);
                case ExpressionType.NegateChecked:
                    return Expression.NegateChecked(operand!, method);
                case ExpressionType.UnaryPlus:
                    return Expression.UnaryPlus(operand!, method);
                case ExpressionType.Convert:
                    return Expression.Convert(operand!, type!, method);
                case ExpressionType.ConvertChecked:
                    return Expression.ConvertChecked(operand!, type!, method);
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

            if (value.Method != null && !value.Method.IsSpecialName)
            {
                WriteMethodInfo(writer, options, value.Method, value.Operand.Type);
            }
        }

        private Expression ReadInvocationExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Expression expression = default;
            var arguments = new List<Expression>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(InvocationExpression.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(InvocationExpression.Arguments), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var argument = JsonSerializer.Deserialize<Expression>(ref reader, options);
                        arguments.Add(argument);
                    }
                }
            }

            return Expression.Invoke(expression!, arguments);
        }

        private void WriteInvocationExpression(Utf8JsonWriter writer, JsonSerializerOptions options, InvocationExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(value.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);

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

        private Expression ReadTypeBinaryExpressions(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Expression expression = default;
            Type typeOperand = default;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(TypeBinaryExpression.Expression), options))
                {
                    expression = JsonSerializer.Deserialize<Expression>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(TypeBinaryExpression.TypeOperand), options))
                {
                    typeOperand = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
            }

            return Expression.TypeIs(expression!, typeOperand!);
        }

        private void WriteTypeBinaryExpressions(Utf8JsonWriter writer, JsonSerializerOptions options, TypeBinaryExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(value.Expression), options));
            JsonSerializer.Serialize(writer, value.Expression, options);

            writer.WritePropertyName(JsonName(nameof(value.TypeOperand), options));
            JsonSerializer.Serialize(writer, value.TypeOperand, options);
        }
        
        private Expression ReadNewExpressions(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Type declaringType = default;
            var arguments = new List<Expression>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, nameof(NewExpression.Constructor.DeclaringType), options))
                {
                    declaringType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(NewExpression.Arguments), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var argument = JsonSerializer.Deserialize<Expression>(ref reader, options);
                        arguments.Add(argument);
                    }
                }
            }

            var ctor = declaringType!.GetConstructor(arguments.Select(x => x.Type).ToArray());
            return Expression.New(ctor!, arguments);
        }

        private void WriteNewExpressions(Utf8JsonWriter writer, JsonSerializerOptions options, NewExpression value)
        {
            writer.WritePropertyName(JsonName(nameof(value.Constructor.DeclaringType), options));
            JsonSerializer.Serialize(writer, value.Constructor.DeclaringType, options);

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

        private Expression ReadNewArrayInitExpressions(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Type elementType = default;
            var expressions = new List<Expression>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                var propertyName = JsonName(reader.GetString(), options);
                reader.Read();

                if (JsonNameEquals(propertyName, "ElementType", options))
                {
                    elementType = JsonSerializer.Deserialize<Type>(ref reader, options);
                }
                else if (JsonNameEquals(propertyName, nameof(NewArrayExpression.Expressions), options))
                {
                    if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var expression = JsonSerializer.Deserialize<Expression>(ref reader, options);
                        expressions.Add(expression);
                    }
                }
            }

            return Expression.NewArrayInit(elementType!, expressions);
        }

        private void WriteNewArrayInitExpressions(Utf8JsonWriter writer, JsonSerializerOptions options, NewArrayExpression value)
        {
            writer.WritePropertyName(JsonName("ElementType", options));
            JsonSerializer.Serialize(writer, value.Type.GetElementType(), options);

            if (value.Expressions.Any())
            {
                writer.WriteStartArray(JsonName(nameof(value.Expressions), options));
                foreach (var expression in value.Expressions)
                {
                    JsonSerializer.Serialize(writer, expression, options);
                }
                writer.WriteEndArray();
            }
        }

        private static void WriteMethodInfo(Utf8JsonWriter writer, JsonSerializerOptions options, MethodInfo value, Type defaultType)
        {
            writer.WriteString(JsonName("MethodName", options), value.Name);

            if (defaultType == null || defaultType != value.DeclaringType)
            {
                writer.WritePropertyName(JsonName(nameof(value.DeclaringType), options));
                JsonSerializer.Serialize(writer, value.DeclaringType, options);
            }
        }

        private static Type GetImpliedDelegateType(LambdaExpression value)
        {
            var parameterTypes = new Type[value.Parameters.Count + 1];
            for (int i = 0; i < value.Parameters.Count; i++)
            {
                var parameter = value.Parameters[i];
                var parameterType = parameter.IsByRef ? parameter.Type.MakeByRefType() : parameter.Type;
                parameterTypes[i] = parameterType;
            }

            parameterTypes[value.Parameters.Count] = value.Body.Type;
            var impliedDelegateType = Expression.GetDelegateType(parameterTypes);
            return impliedDelegateType;
        }
    }
}
