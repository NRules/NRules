﻿using System;
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
        return reader.ReadEnumProperty<ExpressionType>(nameof(Expression.NodeType), options) switch
        {
            ExpressionType.Lambda => ReadLambda(ref reader, options),
            ExpressionType.Parameter => ReadParameter(ref reader, options),
            ExpressionType.Constant => ReadConstant(ref reader, options),
            ExpressionType.MemberAccess => ReadMember(ref reader, options),
            ExpressionType.Call => ReadMethodCall(ref reader, options),
            ExpressionType.Equal => ReadBinary(ref reader, options).ToExpression(Expression.Equal, false),
            ExpressionType.NotEqual => ReadBinary(ref reader, options).ToExpression(Expression.NotEqual, false),
            ExpressionType.LessThanOrEqual => ReadBinary(ref reader, options).ToExpression(Expression.LessThanOrEqual, false),
            ExpressionType.LessThan => ReadBinary(ref reader, options).ToExpression(Expression.LessThan, false),
            ExpressionType.GreaterThanOrEqual => ReadBinary(ref reader, options).ToExpression(Expression.GreaterThanOrEqual, false),
            ExpressionType.GreaterThan => ReadBinary(ref reader, options).ToExpression(Expression.GreaterThan, false),
            ExpressionType.AndAlso => ReadBinary(ref reader, options).ToExpression(Expression.AndAlso),
            ExpressionType.OrElse => ReadBinary(ref reader, options).ToExpression(Expression.OrElse),
            ExpressionType.And => ReadBinary(ref reader, options).ToExpression(Expression.And),
            ExpressionType.Or => ReadBinary(ref reader, options).ToExpression(Expression.Or),
            ExpressionType.ExclusiveOr => ReadBinary(ref reader, options).ToExpression(Expression.ExclusiveOr),
            ExpressionType.Add => ReadBinary(ref reader, options).ToExpression(Expression.Add),
            ExpressionType.AddChecked => ReadBinary(ref reader, options).ToExpression(Expression.AddChecked),
            ExpressionType.Divide => ReadBinary(ref reader, options).ToExpression(Expression.Divide),
            ExpressionType.Modulo => ReadBinary(ref reader, options).ToExpression(Expression.Modulo),
            ExpressionType.Multiply => ReadBinary(ref reader, options).ToExpression(Expression.Multiply),
            ExpressionType.MultiplyChecked => ReadBinary(ref reader, options).ToExpression(Expression.MultiplyChecked),
            ExpressionType.Power => ReadBinary(ref reader, options).ToExpression(Expression.Power),
            ExpressionType.Subtract => ReadBinary(ref reader, options).ToExpression(Expression.Subtract),
            ExpressionType.SubtractChecked => ReadBinary(ref reader, options).ToExpression(Expression.SubtractChecked),
            ExpressionType.Coalesce => ReadBinary(ref reader, options).ToExpression(x => Expression.Coalesce(x.Left, x.Right)),
            ExpressionType.ArrayIndex => ReadBinary(ref reader, options).ToExpression(x => Expression.ArrayIndex(x.Left, x.Right)),
            ExpressionType.LeftShift => ReadBinary(ref reader, options).ToExpression(Expression.LeftShift),
            ExpressionType.RightShift => ReadBinary(ref reader, options).ToExpression(Expression.RightShift),
            ExpressionType.Assign => ReadBinary(ref reader, options).ToExpression(x => Expression.Assign(x.Left, x.Right)),
            ExpressionType.Not => ReadUnary(ref reader, options).ToExpression(Expression.Not),
            ExpressionType.Negate => ReadUnary(ref reader, options).ToExpression(Expression.Negate),
            ExpressionType.NegateChecked => ReadUnary(ref reader, options).ToExpression(Expression.NegateChecked),
            ExpressionType.UnaryPlus => ReadUnary(ref reader, options).ToExpression(Expression.UnaryPlus),
            ExpressionType.Convert => ReadUnary(ref reader, options).ToExpression(Expression.Convert),
            ExpressionType.ConvertChecked => ReadUnary(ref reader, options).ToExpression(Expression.ConvertChecked),
            ExpressionType.TypeAs => ReadUnary(ref reader, options).ToExpression(x => Expression.TypeAs(x.Operand, x.Type)),
            ExpressionType.Invoke => ReadInvocationExpression(ref reader, options),
            ExpressionType.TypeIs => ReadTypeBinaryExpression(ref reader, options),
            ExpressionType.New => ReadNewExpression(ref reader, options),
            ExpressionType.NewArrayInit => ReadNewArrayInitExpression(ref reader, options),
            ExpressionType.MemberInit => ReadMemberInitExpression(ref reader, options),
            ExpressionType.ListInit => ReadListInitExpression(ref reader, options),
            ExpressionType.Conditional => ReadConditionalExpression(ref reader, options),
            ExpressionType.Default => ReadDefaultExpression(ref reader, options),
            ExpressionType.Block => ReadBlockExpression(ref reader, options),
            var type => throw new NotSupportedException($"Unsupported expression type. NodeType={type}"),
        };
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
            case BlockExpression be:
                WriteBlockExpression(writer, options, be);
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

        var expression = type is not null
            ? Expression.Lambda(type, body, parameters)
            : Expression.Lambda(body, parameters);

        var parameterCompactor = new ExpressionParameterCompactor();
        var result = parameterCompactor.Compact(expression);

        return result;
    }

    private static void WriteLambda(Utf8JsonWriter writer, JsonSerializerOptions options, LambdaExpression value)
    {
        var impliedDelegateType = value.GetImpliedDelegateType();

        if (value.Type != impliedDelegateType)
            writer.WriteProperty(nameof(value.Type), value.Type, options);

        if (value.Parameters.Any())
            writer.WriteObjectArrayProperty(nameof(value.Parameters), value.Parameters, options, pe => WriteParameter(writer, options, pe));

        writer.WriteProperty(nameof(value.Body), value.Body, options);
    }

    private static ConstantExpression ReadConstant(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(ConstantExpression.Type), options);
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
        var type = reader.ReadProperty<Type>(nameof(ParameterExpression.Type), options);
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

    private static BinaryMethodData ReadBinary(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var left = reader.ReadProperty<Expression>(nameof(BinaryExpression.Left), options);
        var right = reader.ReadProperty<Expression>(nameof(BinaryExpression.Right), options);
        var method = TryReadMethod(ref reader, options, left, right);
        return new(left, right, method);
    }

    private static MethodInfo TryReadMethod(ref Utf8JsonReader reader, JsonSerializerOptions options, Expression left, Expression right)
    {
        if (!reader.TryReadMethodInfo(options, out var methodRecord))
            return default;

        var argumentTypes = new[] { left.Type, right.Type };
        return methodRecord.GetMethod(argumentTypes, argumentTypes[0]);
    }

    private static MethodInfo TryReadMethod(ref Utf8JsonReader reader, JsonSerializerOptions options, Expression operand)
    {
        if (!reader.TryReadMethodInfo(options, out var methodRecord))
            return default;

        var argumentTypes = new[] { operand.Type };
        return methodRecord.GetMethod(argumentTypes, argumentTypes[0]);
    }

    private static void WriteBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BinaryExpression value)
    {
        writer.WriteProperty(nameof(BinaryExpression.Left), value.Left, options);
        writer.WriteProperty(nameof(BinaryExpression.Right), value.Right, options);

        if (value.Method is not null && !value.Method.IsSpecialName)
            writer.WriteMethodInfo(options, value.Method, value.Left.Type);
    }

    private static UnaryMethodData ReadUnary(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var operand = reader.ReadProperty<Expression>(nameof(UnaryExpression.Operand), options);
        reader.TryReadProperty<Type>(nameof(UnaryExpression.Type), options, out var type);

        var method = TryReadMethod(ref reader, options, operand);
        return new(operand, type, method);
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
        var typeOperand = reader.ReadProperty<Type>(nameof(TypeBinaryExpression.TypeOperand), options);
        return Expression.TypeIs(expression, typeOperand!);
    }

    private static void WriteTypeBinaryExpression(Utf8JsonWriter writer, JsonSerializerOptions options, TypeBinaryExpression value)
    {
        writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        writer.WriteProperty(nameof(value.TypeOperand), value.TypeOperand, options);
    }

    private static NewExpression ReadNewExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var declaringType = reader.ReadProperty<Type>(nameof(NewExpression.Constructor.DeclaringType), options);
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
        return Expression.ListInit(newExpression!, initializers);

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
        var ifTrue = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfTrue), options);
        var ifFalse = reader.ReadProperty<Expression>(nameof(ConditionalExpression.IfFalse), options);
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
        return Expression.Default(type);
    }

    private static void WriteDefaultExpression(Utf8JsonWriter writer, JsonSerializerOptions options, DefaultExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
    }

    private static BlockExpression ReadBlockExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(BlockExpression.Type), options);
        if (type is null)
            throw new JsonException($"Failed to read {nameof(BlockExpression.Type)} property value");

        reader.TryReadObjectArrayProperty(nameof(BlockExpression.Variables), options, ReadParameter, out var variables);
        var expressions = reader.ReadArrayProperty<Expression>(nameof(BlockExpression.Expressions), options);
        return Expression.Block(type, variables, expressions);
    }

    private static void WriteBlockExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BlockExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
        writer.WriteObjectArrayProperty(nameof(value.Variables), value.Variables, options, variable => WriteParameter(writer, options, variable));
        writer.WriteArrayProperty(nameof(value.Expressions), value.Expressions, options);
    }

    private readonly struct BinaryMethodData
    {
        public BinaryMethodData(Expression left, Expression right, MethodInfo method)
        {
            Left = left;
            Right = right;
            Method = method;
        }

        public Expression Left { get; }
        public Expression Right { get; }
        public MethodInfo Method { get; }

        public BinaryExpression ToExpression(Func<BinaryMethodData, BinaryExpression> converter) =>
            converter(this);

        public BinaryExpression ToExpression(Func<Expression, Expression, MethodInfo, BinaryExpression> converter) =>
            converter(Left, Right, Method);

        public BinaryExpression ToExpression(Func<Expression, Expression, bool, MethodInfo, BinaryExpression> converter, bool liftToNull) =>
            converter(Left, Right, liftToNull, Method);
    }

    private readonly struct UnaryMethodData
    {
        public UnaryMethodData(Expression operand, Type type, MethodInfo method)
        {
            Operand = operand;
            Type = type;
            Method = method;
        }

        public Expression Operand { get; }
        public Type Type { get; }
        public MethodInfo Method { get; }

        public UnaryExpression ToExpression(Func<UnaryMethodData, UnaryExpression> converter) =>
            converter(this);

        public UnaryExpression ToExpression(Func<Expression, MethodInfo, UnaryExpression> converter) =>
            converter(Operand, Method);

        public UnaryExpression ToExpression(Func<Expression, Type, MethodInfo, UnaryExpression> converter) =>
            converter(Operand, Type, Method);
    }
}
