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
                return ReadBinary(ref reader, options).ToExpression(Expression.Equal, false);
            case ExpressionType.NotEqual:
                return ReadBinary(ref reader, options).ToExpression(Expression.NotEqual, false);
            case ExpressionType.GreaterThanOrEqual:
                return ReadBinary(ref reader, options).ToExpression(Expression.GreaterThanOrEqual, false);
            case ExpressionType.GreaterThan:
                return ReadBinary(ref reader, options).ToExpression(Expression.GreaterThan, false);
            case ExpressionType.LessThanOrEqual:
                return ReadBinary(ref reader, options).ToExpression(Expression.LessThanOrEqual, false);
            case ExpressionType.LessThan:
                return ReadBinary(ref reader, options).ToExpression(Expression.LessThan, false);
            case ExpressionType.AndAlso:
                return ReadBinary(ref reader, options).ToExpression(Expression.AndAlso);
            case ExpressionType.OrElse:
                return ReadBinary(ref reader, options).ToExpression(Expression.OrElse);
            case ExpressionType.And:
                return ReadBinary(ref reader, options).ToExpression(Expression.And);
            case ExpressionType.Or:
                return ReadBinary(ref reader, options).ToExpression(Expression.Or);
            case ExpressionType.ExclusiveOr:
                return ReadBinary(ref reader, options).ToExpression(Expression.ExclusiveOr);
            case ExpressionType.Add:
                return ReadBinary(ref reader, options).ToExpression(Expression.Add);
            case ExpressionType.AddChecked:
                return ReadBinary(ref reader, options).ToExpression(Expression.AddChecked);
            case ExpressionType.Divide:
                return ReadBinary(ref reader, options).ToExpression(Expression.Divide);
            case ExpressionType.Modulo:
                return ReadBinary(ref reader, options).ToExpression(Expression.Modulo);
            case ExpressionType.Multiply:
                return ReadBinary(ref reader, options).ToExpression(Expression.Multiply);
            case ExpressionType.MultiplyChecked:
                return ReadBinary(ref reader, options).ToExpression(Expression.MultiplyChecked);
            case ExpressionType.Power:
                return ReadBinary(ref reader, options).ToExpression(Expression.Power);
            case ExpressionType.Subtract:
                return ReadBinary(ref reader, options).ToExpression(Expression.Subtract);
            case ExpressionType.SubtractChecked:
                return ReadBinary(ref reader, options).ToExpression(Expression.SubtractChecked);
            case ExpressionType.Coalesce:
                return ReadBinary(ref reader, options).ToExpression(x => Expression.Coalesce(x.Left, x.Right));
            case ExpressionType.ArrayIndex:
                return ReadBinary(ref reader, options).ToExpression(x => Expression.ArrayIndex(x.Left, x.Right));
            case ExpressionType.LeftShift:
                return ReadBinary(ref reader, options).ToExpression(Expression.LeftShift);
            case ExpressionType.RightShift:
                return ReadBinary(ref reader, options).ToExpression(Expression.RightShift);
            case ExpressionType.Assign:
                return ReadBinary(ref reader, options).ToExpression(x => Expression.Assign(x.Left, x.Right));
            case ExpressionType.Not:
                return ReadUnary(ref reader, options).ToExpression(Expression.Not);
            case ExpressionType.Negate:
                return ReadUnary(ref reader, options).ToExpression(Expression.Negate);
            case ExpressionType.NegateChecked:
                return ReadUnary(ref reader, options).ToExpression(Expression.NegateChecked);
            case ExpressionType.UnaryPlus:
                return ReadUnary(ref reader, options).ToExpression(Expression.UnaryPlus);
            case ExpressionType.Convert:
                return ReadUnary(ref reader, options).ToExpression(Expression.Convert);
            case ExpressionType.ConvertChecked:
                return ReadUnary(ref reader, options).ToExpression(Expression.ConvertChecked);
            case ExpressionType.TypeAs:
                return ReadUnary(ref reader, options).ToExpression(x => Expression.TypeAs(x.Operand, x.Type));
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
            case ExpressionType.Block:
                return ReadBlockExpression(ref reader, options);
            default:
                throw new NotSupportedException($"Unsupported expression type. NodeType={nodeType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, Expression value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteEnumProperty(nameof(value.NodeType), value.NodeType, options);

        switch (value.NodeType)
        {
            case ExpressionType.Lambda:
                WriteExpression(writer, options, (LambdaExpression)value);
                break;
            case ExpressionType.Parameter:
                WriteExpression(writer, options, (ParameterExpression)value);
                break;
            case ExpressionType.Constant:
                WriteExpression(writer, options, (ConstantExpression)value);
                break;
            case ExpressionType.MemberAccess:
                WriteExpression(writer, options, (MemberExpression)value);
                break;
            case ExpressionType.Call:
                WriteExpression(writer, options, (MethodCallExpression)value);
                break;
            case ExpressionType.Equal:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.NotEqual:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.GreaterThanOrEqual:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.GreaterThan:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.LessThanOrEqual:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.LessThan:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.AndAlso:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.OrElse:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.And:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Or:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.ExclusiveOr:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Add:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.AddChecked:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Divide:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Modulo:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Multiply:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.MultiplyChecked:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Power:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Subtract:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.SubtractChecked:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Coalesce:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.ArrayIndex:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.LeftShift:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.RightShift:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Assign:
                WriteExpression(writer, options, (BinaryExpression)value);
                break;
            case ExpressionType.Not:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.Negate:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.NegateChecked:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.UnaryPlus:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.Convert:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.ConvertChecked:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.TypeAs:
                WriteExpression(writer, options, (UnaryExpression)value);
                break;
            case ExpressionType.Invoke:
                WriteExpression(writer, options, (InvocationExpression)value);
                break;
            case ExpressionType.TypeIs:
                WriteExpression(writer, options, (TypeBinaryExpression)value);
                break;
            case ExpressionType.New:
                WriteExpression(writer, options, (NewExpression)value);
                break;
            case ExpressionType.NewArrayInit:
                WriteExpression(writer, options, (NewArrayExpression)value);
                break;
            case ExpressionType.MemberInit:
                WriteExpression(writer, options, (MemberInitExpression)value);
                break;
            case ExpressionType.ListInit:
                WriteExpression(writer, options, (ListInitExpression)value);
                break;
            case ExpressionType.Conditional:
                WriteExpression(writer, options, (ConditionalExpression)value);
                break;
            case ExpressionType.Default:
                WriteExpression(writer, options, (DefaultExpression)value);
                break;
            case ExpressionType.Block:
                WriteExpression(writer, options, (BlockExpression)value);
                break;
            default:
                throw new NotSupportedException($"Unsupported expression type. NodeType={value.NodeType}");
        }

        writer.WriteEndObject();
    }

    private LambdaExpression ReadLambda(ref Utf8JsonReader reader, JsonSerializerOptions options)
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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, LambdaExpression value)
    {
        var impliedDelegateType = value.GetImpliedDelegateType();

        if (value.Type != impliedDelegateType)
            writer.WriteProperty(nameof(value.Type), value.Type, options);

        if (value.Parameters.Any())
            writer.WriteObjectArrayProperty(nameof(value.Parameters), value.Parameters, options, pe => WriteExpression(writer, options, pe));

        writer.WriteProperty(nameof(value.Body), value.Body, options);
    }

    private ConstantExpression ReadConstant(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(ConstantExpression.Type), options);
        var value = reader.ReadProperty(nameof(ConstantExpression.Value), type, options);
        return Expression.Constant(value, type);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ConstantExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
        writer.WriteProperty(nameof(value.Value), value.Value, value.Type, options);
    }

    private ParameterExpression ReadParameter(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var name = reader.ReadStringProperty(nameof(ParameterExpression.Name), options);
        var type = reader.ReadProperty<Type>(nameof(ParameterExpression.Type), options);
        return Expression.Parameter(type, name);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ParameterExpression value)
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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, MemberExpression value)
    {
        writer.WriteMemberInfo(options, value.Member);
        if (value.Expression is not null)
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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, MethodCallExpression value)
    {
        if (value.Object is not null)
            writer.WriteProperty(nameof(value.Object), value.Object, options);
        writer.WriteMethodInfo(options, value.Method, value.Object?.Type);
        if (value.Arguments.Any())
            writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
    }

    private BinaryMethodData ReadBinary(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var left = reader.ReadProperty<Expression>(nameof(BinaryExpression.Left), options);
        var right = reader.ReadProperty<Expression>(nameof(BinaryExpression.Right), options);
        var method = TryReadMethod(ref reader, options, left, right);
        return new(left, right, method);
    }

    private MethodInfo TryReadMethod(ref Utf8JsonReader reader, JsonSerializerOptions options, Expression left, Expression right)
    {
        if (!reader.TryReadMethodInfo(options, out var methodRecord))
            return default;

        var argumentTypes = new[] { left.Type, right.Type };
        return methodRecord.GetMethod(argumentTypes, argumentTypes[0]);
    }

    private MethodInfo TryReadMethod(ref Utf8JsonReader reader, JsonSerializerOptions options, Expression operand)
    {
        if (!reader.TryReadMethodInfo(options, out var methodRecord))
            return default;

        var argumentTypes = new[] { operand.Type };
        return methodRecord.GetMethod(argumentTypes, argumentTypes[0]);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BinaryExpression value)
    {
        writer.WriteProperty(nameof(BinaryExpression.Left), value.Left, options);
        writer.WriteProperty(nameof(BinaryExpression.Right), value.Right, options);

        if (value.Method is not null && !value.Method.IsSpecialName)
            writer.WriteMethodInfo(options, value.Method, value.Left.Type);
    }

    private UnaryMethodData ReadUnary(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var operand = reader.ReadProperty<Expression>(nameof(UnaryExpression.Operand), options);
        reader.TryReadProperty<Type>(nameof(UnaryExpression.Type), options, out var type);

        var method = TryReadMethod(ref reader, options, operand);
        return new(operand, type, method);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, UnaryExpression value)
    {
        writer.WriteProperty(nameof(UnaryExpression.Operand), value.Operand, options);
        if (value.Type != value.Operand.Type)
            writer.WriteProperty(nameof(UnaryExpression.Type), value.Type, options);
        if (value.Method is not null && !value.Method.IsSpecialName)
            writer.WriteMethodInfo(options, value.Method, value.Operand.Type);
    }

    private InvocationExpression ReadInvocationExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var expression = reader.ReadProperty<Expression>(nameof(InvocationExpression.Expression), options);
        reader.TryReadArrayProperty<Expression>(nameof(InvocationExpression.Arguments), options, out var arguments);
        return Expression.Invoke(expression, arguments);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, InvocationExpression value)
    {
        writer.WriteProperty(nameof(value.Expression), value.Expression, options);
        if (value.Arguments.Any())
            writer.WriteArrayProperty(nameof(value.Arguments), value.Arguments, options);
    }

    private TypeBinaryExpression ReadTypeBinaryExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var expression = reader.ReadProperty<Expression>(nameof(TypeBinaryExpression.Expression), options);
        var typeOperand = reader.ReadProperty<Type>(nameof(TypeBinaryExpression.TypeOperand), options);
        return Expression.TypeIs(expression, typeOperand!);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, TypeBinaryExpression value)
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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, NewExpression value)
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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, NewArrayExpression value)
    {
        writer.WriteProperty("ElementType", value.Type.GetElementType(), options);
        if (value.Expressions.Any())
            writer.WriteArrayProperty(nameof(value.Expressions), value.Expressions, options);
    }

    private MemberInitExpression ReadMemberInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var newExpression = ReadNewExpression(ref reader, options);
        var bindings = reader.ReadObjectArrayProperty(nameof(MemberInitExpression.Bindings), options, ReadMemberBinding);
        return Expression.MemberInit(newExpression, bindings);

        MemberBinding ReadMemberBinding(ref Utf8JsonReader r, JsonSerializerOptions o)
        {
            return r.ReadMemberBinding(o, newExpression.Type);
        }
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, MemberInitExpression value)
    {
        WriteExpression(writer, options, value.NewExpression);
        writer.WriteObjectArrayProperty(nameof(value.Bindings), value.Bindings, options, mb =>
        {
            writer.WriteMemberBinding(mb, options, value.NewExpression.Type);
        });
    }

    private ListInitExpression ReadListInitExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ListInitExpression value)
    {
        WriteExpression(writer, options, value.NewExpression);

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

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, ConditionalExpression value)
    {
        writer.WriteProperty(nameof(value.Test), value.Test, options);
        writer.WriteProperty(nameof(value.IfTrue), value.IfTrue, options);
        writer.WriteProperty(nameof(value.IfFalse), value.IfFalse, options);
    }

    private DefaultExpression ReadDefaultExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(DefaultExpression.Type), options);
        return Expression.Default(type);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, DefaultExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
    }

    private BlockExpression ReadBlockExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var type = reader.ReadProperty<Type>(nameof(BlockExpression.Type), options);
        if (type is null)
            throw new JsonException($"Failed to read {nameof(BlockExpression.Type)} property value");

        reader.TryReadObjectArrayProperty(nameof(BlockExpression.Variables), options, ReadParameter, out var variables);
        var expressions = reader.ReadArrayProperty<Expression>(nameof(BlockExpression.Expressions), options);
        return Expression.Block(type, variables, expressions);
    }

    private void WriteExpression(Utf8JsonWriter writer, JsonSerializerOptions options, BlockExpression value)
    {
        writer.WriteProperty(nameof(value.Type), value.Type, options);
        writer.WriteObjectArrayProperty(nameof(value.Variables), value.Variables, options, variable => WriteExpression(writer, options, variable));
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
