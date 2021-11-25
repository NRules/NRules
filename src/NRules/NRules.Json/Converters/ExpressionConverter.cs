using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;

namespace NRules.Json.Converters
{
    internal class ExpressionConverter : JsonConverter<Expression>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(Expression).IsAssignableFrom(typeToConvert);

        public override Expression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.ReadStartObject();
            var nodeType = reader.ReadEnumProperty<ExpressionType>(nameof(Expression.NodeType), options);

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
                value = ReadTypeBinaryExpression(ref reader, options);
            else if (nodeType == ExpressionType.New)
                value = ReadNewExpression(ref reader, options);
            else if (nodeType == ExpressionType.NewArrayInit)
                value = ReadNewArrayInitExpression(ref reader, options);
            else if (nodeType == ExpressionType.MemberInit)
                value = ReadMemberInitExpression(ref reader, options);
            else if (nodeType == ExpressionType.ListInit)
                value = ReadListInitExpression(ref reader, options);
            else if (nodeType == ExpressionType.Conditional)
                value = ReadConditionalExpression(ref reader, options);
            else
                throw new NotSupportedException($"Unsupported expression type. NodeType={nodeType}");

            return value;
        }

        public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteEnumProperty(nameof(value.NodeType), value.NodeType, options);

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
                WriteTypeBinaryExpression(writer, options, tbe);
            else if (value is NewExpression ne)
                WriteNewExpression(writer, options, ne);
            else if (value is NewArrayExpression nae)
                WriteNewArrayInitExpression(writer, options, nae);
            else if (value is MemberInitExpression mie)
                WriteMemberInitExpression(writer, options, mie);
            else if (value is ListInitExpression lie)
                WriteListInitExpression(writer, options, lie);
            else if (value is ConditionalExpression cne)
                WriteConditionalExpression(writer, options, cne);
            else
                throw new NotSupportedException($"Unsupported expression type. NodeType={value.NodeType}");

            writer.WriteEndObject();
        }
        
        private LambdaExpression ReadLambda(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            reader.TryReadProperty<Type>(nameof(LambdaExpression.Type), options, out var type);
            var body = reader.ReadProperty<Expression>(nameof(LambdaExpression.Body), options);
            reader.TryReadArrayProperty<ParameterExpression>(nameof(LambdaExpression.Parameters), options, out var parameters);

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
                writer.WriteProperty(nameof(value.Type), value.Type, options);

            writer.WriteProperty(nameof(value.Body), value.Body, options);

            if (value.Parameters.Any())
                writer.WriteArrayProperty(nameof(value.Parameters), value.Parameters, options);
        }

        private ConstantExpression ReadConstant(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var type = reader.ReadProperty<Type>(nameof(ConstantExpression.Type), options);
            var value = reader.ReadProperty(nameof(ConstantExpression.Value), type, options);
            return Expression.Constant(value, type!);
        }

        private void WriteConstant(Utf8JsonWriter writer, JsonSerializerOptions options, ConstantExpression value)
        {
            writer.WriteProperty(nameof(value.Type), value.Type, options);
            writer.WriteProperty(nameof(value.Value), value.Value, value.Type, options);
        }

        private ParameterExpression ReadParameter(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var name = reader.ReadStringProperty(nameof(ParameterExpression.Name), options);
            var type = reader.ReadProperty<Type>(nameof(ParameterExpression.Type), options);
            return Expression.Parameter(type!, name);
        }

        private void WriteParameter(Utf8JsonWriter writer, JsonSerializerOptions options, ParameterExpression value)
        {
            writer.WriteStringProperty(nameof(value.Name), value.Name, options);
            writer.WriteProperty(nameof(value.Type), value.Type, options);
        }

        private MemberExpression ReadMember(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var member = reader.ReadMemberInfo(options);
            reader.TryReadProperty<Expression>(nameof(MemberExpression.Expression), options, out var expression);
            return Expression.MakeMemberAccess(expression, member);
        }

        private void WriteMember(Utf8JsonWriter writer, JsonSerializerOptions options, MemberExpression value)
        {
            writer.WriteMemberInfo(options, value.Member);
            if (value.Expression != null)
                writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        }

        private MethodCallExpression ReadMethodCall(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            reader.TryReadProperty<Expression>(nameof(MethodCallExpression.Object), options, out var @object);
            var methodRecord = reader.ReadMethodInfo(options);
            reader.TryReadArrayProperty<Expression>(nameof(MethodCallExpression.Arguments), options, out var arguments);
            var method = methodRecord.GetMethod(arguments.Select(x => x.Type).ToArray(), @object?.Type);
            return Expression.Call(@object, method, arguments);
        }

        private void WriteMethodCall(Utf8JsonWriter writer, JsonSerializerOptions options, MethodCallExpression value)
        {
            if (value.Object != null)
                writer.WriteProperty(nameof(value.Object), value.Object, options);
            writer.WriteMethodInfo(options, value.Method, value.Object?.Type);
            if (value.Arguments.Any())
                writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
        }

        private BinaryExpression ReadBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
        {
            var left = reader.ReadProperty<Expression>(nameof(BinaryExpression.Left), options);
            var right = reader.ReadProperty<Expression>(nameof(BinaryExpression.Right), options);

            MethodInfo method = default;
            if (reader.TryReadMethodInfo(options, out var methodRecord))
                method = methodRecord.GetMethod(new[] { left.Type, right.Type }, left.Type);
            
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return Expression.Equal(left, right, false, method);
                case ExpressionType.NotEqual:
                    return Expression.NotEqual(left, right, false, method);
                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right, false, method);
                case ExpressionType.LessThan:
                    return Expression.LessThan(left, right, false, method);
                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right, false, method);
                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(left, right, false, method);
                case ExpressionType.AndAlso:
                    return Expression.AndAlso(left, right, method);
                case ExpressionType.OrElse:
                    return Expression.OrElse(left, right, method);
                case ExpressionType.And:
                    return Expression.And(left, right, method);
                case ExpressionType.Or:
                    return Expression.Or(left, right, method);
                case ExpressionType.ExclusiveOr:
                    return Expression.ExclusiveOr(left, right, method);
                case ExpressionType.Add:
                    return Expression.Add(left, right, method);
                case ExpressionType.AddChecked:
                    return Expression.AddChecked(left, right, method);
                case ExpressionType.Divide:
                    return Expression.Divide(left, right, method);
                case ExpressionType.Modulo:
                    return Expression.Modulo(left, right, method);
                case ExpressionType.Multiply:
                    return Expression.Multiply(left, right, method);
                case ExpressionType.MultiplyChecked:
                    return Expression.MultiplyChecked(left, right, method);
                case ExpressionType.Power:
                    return Expression.Power(left, right, method);
                case ExpressionType.Subtract:
                    return Expression.Subtract(left, right, method);
                case ExpressionType.SubtractChecked:
                    return Expression.SubtractChecked(left, right, method);
                case ExpressionType.Coalesce:
                    return Expression.Coalesce(left, right);
                case ExpressionType.ArrayIndex:
                    return Expression.ArrayIndex(left, right);
                case ExpressionType.LeftShift:
                    return Expression.LeftShift(left, right, method);
                case ExpressionType.RightShift:
                    return Expression.RightShift(left, right, method);
                default:
                    throw new NotSupportedException($"Unrecognized binary expression: {expressionType}");
            }
        }

        private void WriteBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BinaryExpression value)
        {
            writer.WriteProperty(nameof(BinaryExpression.Left), value.Left, options);
            writer.WriteProperty(nameof(BinaryExpression.Right), value.Right, options);

            if (value.Method != null && !value.Method.IsSpecialName)
                writer.WriteMethodInfo(options, value.Method, value.Left.Type);
        }

        private UnaryExpression ReadUnaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
        {
            var operand = reader.ReadProperty<Expression>(nameof(UnaryExpression.Operand), options);
            reader.TryReadProperty<Type>(nameof(UnaryExpression.Type), options, out var type);

            MethodInfo method = default;
            if (reader.TryReadMethodInfo(options, out var methodRecord))
                method = methodRecord.GetMethod(new[] { operand.Type }, operand.Type);

            switch (expressionType)
            {
                case ExpressionType.Not:
                    return Expression.Not(operand, method);
                case ExpressionType.Negate:
                    return Expression.Negate(operand, method);
                case ExpressionType.NegateChecked:
                    return Expression.NegateChecked(operand, method);
                case ExpressionType.UnaryPlus:
                    return Expression.UnaryPlus(operand, method);
                case ExpressionType.Convert:
                    return Expression.Convert(operand, type, method);
                case ExpressionType.ConvertChecked:
                    return Expression.ConvertChecked(operand, type, method);
                case ExpressionType.TypeAs:
                    return Expression.TypeAs(operand, type);
                default:
                    throw new NotSupportedException($"Unrecognized unary expression: {expressionType}");
            }
        }

        private void WriteUnaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, UnaryExpression value)
        {
            writer.WriteProperty(nameof(UnaryExpression.Operand), value.Operand, options);
            if (value.Type != value.Operand.Type)
                writer.WriteProperty(nameof(UnaryExpression.Type), value.Type, options);
            if (value.Method != null && !value.Method.IsSpecialName)
                writer.WriteMethodInfo(options, value.Method, value.Operand.Type);
        }

        private InvocationExpression ReadInvocationExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var expression = reader.ReadProperty<Expression>(nameof(InvocationExpression.Expression), options);
            reader.TryReadArrayProperty<Expression>(nameof(InvocationExpression.Arguments), options, out var arguments);
            return Expression.Invoke(expression!, arguments);
        }

        private void WriteInvocationExpression(Utf8JsonWriter writer, JsonSerializerOptions options, InvocationExpression value)
        {
            writer.WriteProperty(nameof(value.Expression), value.Expression, options);
            if (value.Arguments.Any())
                writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
        }

        private TypeBinaryExpression ReadTypeBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var expression = reader.ReadProperty<Expression>(nameof(TypeBinaryExpression.Expression), options);
            var typeOperand = reader.ReadProperty<Type>(nameof(TypeBinaryExpression.TypeOperand), options); 
            return Expression.TypeIs(expression!, typeOperand!);
        }

        private void WriteTypeBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, TypeBinaryExpression value)
        {
            writer.WriteProperty(nameof(value.Expression), value.Expression, options);
            writer.WriteProperty(nameof(value.TypeOperand), value.TypeOperand, options);
        }
        
        private NewExpression ReadNewExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var declaringType = reader.ReadProperty<Type>(nameof(NewExpression.Constructor.DeclaringType), options);
            reader.TryReadArrayProperty<Expression>(nameof(NewExpression.Arguments), options, out var arguments);
            
            var ctor = declaringType.GetConstructor(arguments.Select(x => x.Type).ToArray())
                ?? throw new ArgumentException($"Unable to find constructor. Type={declaringType}", nameof(declaringType));
            return Expression.New(ctor, arguments);
        }

        private void WriteNewExpression(Utf8JsonWriter writer, JsonSerializerOptions options, NewExpression value)
        {
            writer.WriteProperty(nameof(value.Constructor.DeclaringType), value.Constructor.DeclaringType, options);
            if (value.Arguments.Any())
                writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
        }

        private NewArrayExpression ReadNewArrayInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var elementType = reader.ReadProperty<Type>("ElementType", options);
            reader.TryReadArrayProperty<Expression>(nameof(NewArrayExpression.Expressions), options, out var expressions);
            return Expression.NewArrayInit(elementType, expressions);
        }

        private void WriteNewArrayInitExpression(Utf8JsonWriter writer, JsonSerializerOptions options, NewArrayExpression value)
        {
            writer.WriteProperty("ElementType", value.Type.GetElementType(), options);
            if (value.Expressions.Any())
                writer.WriteArrayProperty(nameof(value.Expressions), value.Expressions, options);
        }

        private MemberInitExpression ReadMemberInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var newExpression = ReadNewExpression(ref reader, options);
            var bindings = reader.ReadArrayProperty<MemberBinding>(nameof(MemberInitExpression.Bindings), options);
            return Expression.MemberInit(newExpression, bindings);
        }

        private void WriteMemberInitExpression(Utf8JsonWriter writer, JsonSerializerOptions options, MemberInitExpression value)
        {
            WriteNewExpression(writer, options, value.NewExpression);
            writer.WriteArrayProperty(nameof(value.Bindings), value.Bindings, options);
        }

        private ListInitExpression ReadListInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var newExpression = ReadNewExpression(ref reader, options);
            var initializers = reader.ReadObjectArrayProperty(nameof(ListInitExpression.Initializers), options, ReadElementInit);
            return Expression.ListInit(newExpression!, initializers);

            ElementInit ReadElementInit(ref Utf8JsonReader r)
            {
                var methodRecord = r.ReadMethodInfo(options);
                var arguments = r.ReadArrayProperty<Expression>(nameof(ElementInit.Arguments), options);
                var method = methodRecord.GetMethod(arguments.Select(x => x.Type).ToArray(), newExpression.Type);
                return Expression.ElementInit(method, arguments);
            }
        }
        
        private void WriteListInitExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ListInitExpression value)
        {
            WriteNewExpression(writer, options, value.NewExpression);

            writer.WriteObjectArrayProperty(nameof(value.Initializers), value.Initializers, options, initializer =>
            {
                writer.WriteMethodInfo(options, initializer.AddMethod, value.Type);
                writer.WriteArrayProperty(nameof(initializer.Arguments), initializer.Arguments, options);
            });
        }
        
        private ConditionalExpression ReadConditionalExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var test = reader.ReadProperty<Expression>(nameof(ConditionalExpression.Test), options);
            var ifTrue = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfTrue), options);
            var ifFalse = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfFalse), options);
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        private void WriteConditionalExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ConditionalExpression value)
        {
            writer.WriteProperty(nameof(value.Test), value.Test, options);
            writer.WriteProperty(nameof(value.IfTrue), value.IfTrue, options);
            writer.WriteProperty(nameof(value.IfFalse), value.IfFalse, options);
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
