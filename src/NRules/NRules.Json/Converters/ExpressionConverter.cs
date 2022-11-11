using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRules.Json.Utilities;

namespace NRules.Json.Converters;

internal class ExpressionConverter : JsonConverter<Expression>
{
    public override bool CanConvert(Type typeToConvert) =>
        typeof(Expression).IsAssignableFrom(typeToConvert);

    public override Expression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.ReadStartObject();
        var nodeType = reader.ReadEnumProperty<ExpressionType>(nameof(Expression.NodeType), options);

        switch (nodeType)
        {
            case ExpressionType.Lambda:
                return ReadLambda(ref reader, options);
            case ExpressionType.Parameter:
                return ReadParameter(ref reader, options);
            case ExpressionType.Constant:
                return ReadConstant(ref reader, options);
            case ExpressionType.MemberAccess:
                return ReadMember(ref reader, options);
            case ExpressionType.Call:
                return ReadMethodCall(ref reader, options);
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
            case ExpressionType.And:
            case ExpressionType.Or:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Power:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
            case ExpressionType.LeftShift:
            case ExpressionType.RightShift:
                return ReadBinaryExpression(ref reader, options, nodeType);
            case ExpressionType.Not:
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.UnaryPlus:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.TypeAs:
                return ReadUnaryExpression(ref reader, options, nodeType);
            case ExpressionType.Invoke:
                return ReadInvocationExpression(ref reader, options);
            case ExpressionType.TypeIs:
                return ReadTypeBinaryExpression(ref reader, options);
            case ExpressionType.New:
                return ReadNewExpression(ref reader, options);
            case ExpressionType.NewArrayInit:
                return ReadNewArrayInitExpression(ref reader, options);
            case ExpressionType.MemberInit:
                return ReadMemberInitExpression(ref reader, options);
            case ExpressionType.ListInit:
                return ReadListInitExpression(ref reader, options);
            case ExpressionType.Conditional:
                return ReadConditionalExpression(ref reader, options);
            case ExpressionType.Default:
                return ReadDefaultExpression(ref reader, options);
            default:
                throw new NotSupportedException($"Unsupported expression type. NodeType={nodeType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteEnumProperty(nameof(value.NodeType), value.NodeType, options);

        switch (value)
        {
            case LambdaExpression le:
                WriteLambda(writer, options, le);
                break;
            case ParameterExpression pe:
                WriteParameter(writer, options, pe);
                break;
            case ConstantExpression ce:
                WriteConstant(writer, options, ce);
                break;
            case MemberExpression me:
                WriteMember(writer, options, me);
                break;
            case MethodCallExpression mce:
                WriteMethodCall(writer, options, mce);
                break;
            case BinaryExpression be:
                WriteBinaryExpression(writer, options, be);
                break;
            case UnaryExpression ue:
                WriteUnaryExpression(writer, options, ue);
                break;
            case InvocationExpression ie:
                WriteInvocationExpression(writer, options, ie);
                break;
            case TypeBinaryExpression tbe:
                WriteTypeBinaryExpression(writer, options, tbe);
                break;
            case NewExpression ne:
                WriteNewExpression(writer, options, ne);
                break;
            case NewArrayExpression nae:
                WriteNewArrayInitExpression(writer, options, nae);
                break;
            case MemberInitExpression mie:
                WriteMemberInitExpression(writer, options, mie);
                break;
            case ListInitExpression lie:
                WriteListInitExpression(writer, options, lie);
                break;
            case ConditionalExpression ce:
                WriteConditionalExpression(writer, options, ce);
                break;
            case DefaultExpression de:
                WriteDefaultExpression(writer, options, de);
                break;
            default:
                throw new NotSupportedException($"Unsupported expression type. NodeType={value.NodeType}");
        }

        writer.WriteEndObject();
    }

    private static LambdaExpression ReadLambda(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.TryReadProperty<Type>(nameof(LambdaExpression.Type), options, out var type);
        reader.TryReadObjectArrayProperty(nameof(LambdaExpression.Parameters), options, ReadParameter, out var parameters);
        var body = reader.ReadProperty<Expression>(nameof(LambdaExpression.Body), options);
        if (body is null)
        {
            throw new JsonException($"Failed to read {nameof(LambdaExpression.Body)} property value");
        }

        var expression = type is not null
            ? Expression.Lambda(type, body, parameters)
            : Expression.Lambda(body, parameters);

        var parameterCompactor = new ExpressionParameterCompactor();
        var result = parameterCompactor.Compact(expression);

        return result;
    }

    private static void WriteLambda(Utf8JsonWriter writer, JsonSerializerOptions options, LambdaExpression value)
    {
        if (value.Type != value.GetImpliedDelegateType())
            writer.WriteProperty(nameof(value.Type), value.Type, options);
        if (value.Parameters.Any())
            writer.WriteObjectArrayProperty(nameof(value.Parameters), value.Parameters, options, pe =>
            {
                WriteParameter(writer, options, pe);
            });
        writer.WriteProperty(nameof(value.Body), value.Body, options);
    }

    private static ConstantExpression ReadConstant(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(ConstantExpression.Type), options);
        if (type is null)
        {
            throw new JsonException($"Failed to read {nameof(ConstantExpression.Type)} property value");
        }
        var value = reader.ReadProperty(nameof(ConstantExpression.Value), type, options);
        return Expression.Constant(value, type);
    }

    private static void WriteConstant(Utf8JsonWriter writer, JsonSerializerOptions options, ConstantExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
        writer.WriteProperty(nameof(value.Value), value.Value, value.Type, options);
    }

    private static ParameterExpression ReadParameter(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty(nameof(ParameterExpression.Name), options);
        if (name is null)
        {
            throw new JsonException($"Failed to read {nameof(ParameterExpression.Name)} property value");
        }
        var type = reader.ReadProperty<Type>(nameof(ParameterExpression.Type), options);
        if (type is null)
        {
            throw new JsonException($"Failed to read {nameof(ParameterExpression.Type)} property value");
        }
        return Expression.Parameter(type, name);
    }

    private static void WriteParameter(Utf8JsonWriter writer, JsonSerializerOptions options, ParameterExpression value)
    {
        writer.WriteStringProperty(nameof(value.Name), value.Name, options);
        writer.WriteProperty(nameof(value.Type), value.Type, options);
    }

    private static MemberExpression ReadMember(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var member = reader.ReadMemberInfo(options);
        reader.TryReadProperty<Expression>(nameof(MemberExpression.Expression), options, out var expression);
        return Expression.MakeMemberAccess(expression, member);
    }

    private static void WriteMember(Utf8JsonWriter writer, JsonSerializerOptions options, MemberExpression value)
    {
        writer.WriteMemberInfo(options, value.Member);
        if (value.Expression is not null)
            writer.WriteProperty(nameof(value.Expression), value.Expression, options);
    }

    private static MethodCallExpression ReadMethodCall(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.TryReadProperty<Expression>(nameof(MethodCallExpression.Object), options, out var @object);
        var methodRecord = reader.ReadMethodInfo(options);
        reader.TryReadArrayProperty<Expression>(nameof(MethodCallExpression.Arguments), options, out var arguments);
        var method = methodRecord.GetMethod(arguments.Select(x => x.Type).ToArray(), @object?.Type);
        return Expression.Call(@object, method, arguments);
    }

    private static void WriteMethodCall(Utf8JsonWriter writer, JsonSerializerOptions options, MethodCallExpression value)
    {
        if (value.Object is not null)
            writer.WriteProperty(nameof(value.Object), value.Object, options);
        writer.WriteMethodInfo(options, value.Method, value.Object?.Type);
        if (value.Arguments.Any())
            writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
    }

    private static BinaryExpression ReadBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
    {
        var left = reader.ReadProperty<Expression>(nameof(BinaryExpression.Left), options);
        if (left is null)
        {
            throw new JsonException($"Failed to read {nameof(BinaryExpression.Left)} property value");
        }
        var right = reader.ReadProperty<Expression>(nameof(BinaryExpression.Right), options);
        if (right is null)
        {
            throw new JsonException($"Failed to read {nameof(BinaryExpression.Right)} property value");
        }

        MethodInfo? method = default;
        if (reader.TryReadMethodInfo(options, out var methodRecord))
            method = methodRecord.GetMethod(new[] { left.Type, right.Type }, left.Type);

        return expressionType switch
        {
            ExpressionType.Equal => Expression.Equal(left, right, false, method),
            ExpressionType.NotEqual => Expression.NotEqual(left, right, false, method),
            ExpressionType.LessThanOrEqual => Expression.LessThanOrEqual(left, right, false, method),
            ExpressionType.LessThan => Expression.LessThan(left, right, false, method),
            ExpressionType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right, false, method),
            ExpressionType.GreaterThan => Expression.GreaterThan(left, right, false, method),
            ExpressionType.AndAlso => Expression.AndAlso(left, right, method),
            ExpressionType.OrElse => Expression.OrElse(left, right, method),
            ExpressionType.And => Expression.And(left, right, method),
            ExpressionType.Or => Expression.Or(left, right, method),
            ExpressionType.ExclusiveOr => Expression.ExclusiveOr(left, right, method),
            ExpressionType.Add => Expression.Add(left, right, method),
            ExpressionType.AddChecked => Expression.AddChecked(left, right, method),
            ExpressionType.Divide => Expression.Divide(left, right, method),
            ExpressionType.Modulo => Expression.Modulo(left, right, method),
            ExpressionType.Multiply => Expression.Multiply(left, right, method),
            ExpressionType.MultiplyChecked => Expression.MultiplyChecked(left, right, method),
            ExpressionType.Power => Expression.Power(left, right, method),
            ExpressionType.Subtract => Expression.Subtract(left, right, method),
            ExpressionType.SubtractChecked => Expression.SubtractChecked(left, right, method),
            ExpressionType.Coalesce => Expression.Coalesce(left, right),
            ExpressionType.ArrayIndex => Expression.ArrayIndex(left, right),
            ExpressionType.LeftShift => Expression.LeftShift(left, right, method),
            ExpressionType.RightShift => Expression.RightShift(left, right, method),
            _ => throw new NotSupportedException($"Unrecognized binary expression: {expressionType}")
        };
    }

    private static void WriteBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BinaryExpression value)
    {
        writer.WriteProperty(nameof(BinaryExpression.Left), value.Left, options);
        writer.WriteProperty(nameof(BinaryExpression.Right), value.Right, options);

        if (value.Method is not null && !value.Method.IsSpecialName)
            writer.WriteMethodInfo(options, value.Method, value.Left.Type);
    }

    private static UnaryExpression ReadUnaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options, ExpressionType expressionType)
    {
        var operand = reader.ReadProperty<Expression>(nameof(UnaryExpression.Operand), options);
        if (operand is null)
        {
            throw new JsonException($"Failed to read {nameof(UnaryExpression.Operand)} property value");
        }

        reader.TryReadProperty<Type>(nameof(UnaryExpression.Type), options, out var type);

        MethodInfo? method = default;
        if (reader.TryReadMethodInfo(options, out var methodRecord))
            method = methodRecord.GetMethod(new[] { operand.Type }, operand.Type);

        return expressionType switch
        {
            ExpressionType.Not => Expression.Not(operand, method),
            ExpressionType.Negate => Expression.Negate(operand, method),
            ExpressionType.NegateChecked => Expression.NegateChecked(operand, method),
            ExpressionType.UnaryPlus => Expression.UnaryPlus(operand, method),
            ExpressionType.Convert => Expression.Convert(operand, type ?? throw new JsonException($"Failed to read {nameof(UnaryExpression.Type)} property value"), method),
            ExpressionType.ConvertChecked => Expression.ConvertChecked(operand, type ?? throw new JsonException($"Failed to read {nameof(UnaryExpression.Type)} property value"), method),
            ExpressionType.TypeAs => Expression.TypeAs(operand, type ?? throw new JsonException($"Failed to read {nameof(UnaryExpression.Type)} property value")),
            _ => throw new NotSupportedException($"Unrecognized unary expression: {expressionType}")
        };
    }

    private static void WriteUnaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, UnaryExpression value)
    {
        writer.WriteProperty(nameof(UnaryExpression.Operand), value.Operand, options);
        if (value.Type != value.Operand.Type)
            writer.WriteProperty(nameof(UnaryExpression.Type), value.Type, options);
        if (value.Method is not null && !value.Method.IsSpecialName)
            writer.WriteMethodInfo(options, value.Method, value.Operand.Type);
    }

    private static InvocationExpression ReadInvocationExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var expression = reader.ReadProperty<Expression>(nameof(InvocationExpression.Expression), options);
        if (expression is null)
        {
            throw new JsonException($"Failed to read {nameof(InvocationExpression.Expression)} property value");
        }
        reader.TryReadArrayProperty<Expression>(nameof(InvocationExpression.Arguments), options, out var arguments);
        return Expression.Invoke(expression, arguments);
    }

    private static void WriteInvocationExpression(Utf8JsonWriter writer, JsonSerializerOptions options, InvocationExpression value)
    {
        writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        if (value.Arguments.Any())
            writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
    }

    private static TypeBinaryExpression ReadTypeBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var expression = reader.ReadProperty<Expression>(nameof(TypeBinaryExpression.Expression), options);
        if (expression is null)
        {
            throw new JsonException($"Failed to read {nameof(TypeBinaryExpression.Expression)} property value");
        }
        var typeOperand = reader.ReadProperty<Type>(nameof(TypeBinaryExpression.TypeOperand), options);
        return Expression.TypeIs(expression, typeOperand);
    }

    private static void WriteTypeBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, TypeBinaryExpression value)
    {
        writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        writer.WriteProperty(nameof(value.TypeOperand), value.TypeOperand, options);
    }

    private static NewExpression ReadNewExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var declaringType = reader.ReadProperty<Type>(nameof(NewExpression.Constructor.DeclaringType), options);
        if (declaringType is null)
        {
            throw new JsonException($"Failed to read {nameof(NewExpression.Constructor.DeclaringType)} property value");
        }
        reader.TryReadArrayProperty<Expression>(nameof(NewExpression.Arguments), options, out var arguments);

        var ctor = declaringType.GetConstructor(arguments.Select(x => x.Type).ToArray())
            ?? throw new ArgumentException($"Unable to find constructor. Type={declaringType}", nameof(declaringType));
        return Expression.New(ctor, arguments);
    }

    private static void WriteNewExpression(Utf8JsonWriter writer, JsonSerializerOptions options, NewExpression value)
    {
        writer.WriteProperty(nameof(value.Constructor.DeclaringType), value.Constructor.DeclaringType, options);
        if (value.Arguments.Any())
            writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
    }

    private static NewArrayExpression ReadNewArrayInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var elementType = reader.ReadProperty<Type>("ElementType", options);
        if (elementType is null)
        {
            throw new JsonException("Failed to read ElementType property value");
        }
        reader.TryReadArrayProperty<Expression>(nameof(NewArrayExpression.Expressions), options, out var expressions);
        return Expression.NewArrayInit(elementType, expressions);
    }

    private static void WriteNewArrayInitExpression(Utf8JsonWriter writer, JsonSerializerOptions options, NewArrayExpression value)
    {
        writer.WriteProperty("ElementType", value.Type.GetElementType(), options);
        if (value.Expressions.Any())
            writer.WriteArrayProperty(nameof(value.Expressions), value.Expressions, options);
    }

    private static MemberInitExpression ReadMemberInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var newExpression = ReadNewExpression(ref reader, options);
        var bindings = reader.ReadObjectArrayProperty(nameof(MemberInitExpression.Bindings), options, ReadMemberBinding);
        return Expression.MemberInit(newExpression, bindings);

        MemberBinding ReadMemberBinding(ref Utf8JsonReader r, JsonSerializerOptions o)
        {
            return r.ReadMemberBinding(o, newExpression.Type);
        }
    }

    private static void WriteMemberInitExpression(Utf8JsonWriter writer, JsonSerializerOptions options, MemberInitExpression value)
    {
        WriteNewExpression(writer, options, value.NewExpression);
        writer.WriteObjectArrayProperty(nameof(value.Bindings), value.Bindings, options, mb =>
        {
            writer.WriteMemberBinding(mb, options, value.NewExpression.Type);
        });
    }

    private static ListInitExpression ReadListInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var newExpression = ReadNewExpression(ref reader, options);
        var initializers = reader.ReadObjectArrayProperty(nameof(ListInitExpression.Initializers), options, ReadElementInit);
        return Expression.ListInit(newExpression, initializers);

        ElementInit ReadElementInit(ref Utf8JsonReader r, JsonSerializerOptions o)
        {
            var methodRecord = r.ReadMethodInfo(o);
            var arguments = r.ReadArrayProperty<Expression>(nameof(ElementInit.Arguments), o);
            var method = methodRecord.GetMethod(arguments.Select(x => x.Type).ToArray(), newExpression.Type);
            return Expression.ElementInit(method, arguments);
        }
    }

    private static void WriteListInitExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ListInitExpression value)
    {
        WriteNewExpression(writer, options, value.NewExpression);

        writer.WriteObjectArrayProperty(nameof(value.Initializers), value.Initializers, options, initializer =>
        {
            writer.WriteMethodInfo(options, initializer.AddMethod, value.Type);
            writer.WriteArrayProperty(nameof(initializer.Arguments), initializer.Arguments, options);
        });
    }

    private static ConditionalExpression ReadConditionalExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var test = reader.ReadProperty<Expression>(nameof(ConditionalExpression.Test), options);
        if (test is null)
        {
            throw new JsonException($"Failed to read {nameof(ConditionalExpression.Test)} property value");
        }
        var ifTrue = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfTrue), options);
        if (ifTrue is null)
        {
            throw new JsonException($"Failed to read {nameof(ConditionalExpression.IfTrue)} property value");
        }
        var ifFalse = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfFalse), options);
        if (ifFalse is null)
        {
            throw new JsonException($"Failed to read {nameof(ConditionalExpression.IfFalse)} property value");
        }
        return Expression.Condition(test, ifTrue, ifFalse);
    }

    private static void WriteConditionalExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ConditionalExpression value)
    {
        writer.WriteProperty(nameof(value.Test), value.Test, options);
        writer.WriteProperty(nameof(value.IfTrue), value.IfTrue, options);
        writer.WriteProperty(nameof(value.IfFalse), value.IfFalse, options);
    }

    private static DefaultExpression ReadDefaultExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(DefaultExpression.Type), options);
        if (type is null)
        {
            throw new JsonException($"Failed to read {nameof(DefaultExpression.Type)} property value");
        }
        return Expression.Default(type);
    }

    private static void WriteDefaultExpression(Utf8JsonWriter writer, JsonSerializerOptions options, DefaultExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
    }
}
