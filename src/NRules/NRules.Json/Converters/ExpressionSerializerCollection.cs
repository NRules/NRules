using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Json.Converters;

internal interface IExpressionSerializerCollection
{
    IExpressionSerializer GetSerializer(ExpressionType type);
}

internal sealed class ExpressionSerializerCollection : IExpressionSerializerCollection
{
    private static readonly IReadOnlyDictionary<ExpressionType, IExpressionSerializer> _serializers = GetSerializers().ToDictionary(s => s.SupportedType);

    public IExpressionSerializer GetSerializer(ExpressionType type)
    {
        if (!_serializers.TryGetValue(type, out IExpressionSerializer serializer))
            throw new NotSupportedException($"Unsupported expression type. NodeType={type}");

        return serializer;
    }

    private static IEnumerable<IExpressionSerializer> GetSerializers()
    {
        var parameter = new ParameterExpressionSerializer();
        yield return parameter;
        yield return new LambdaExpressionSerializer(parameter);
        yield return new ConstantExpressionSerializer();
        yield return new MemberAccessExpressionSerializer();
        yield return new CallExpressionSerializer();

        yield return new BinaryExpressionSerializer(ExpressionType.Equal, Expression.Equal, false);
        yield return new BinaryExpressionSerializer(ExpressionType.NotEqual, Expression.NotEqual, false);
        yield return new BinaryExpressionSerializer(ExpressionType.GreaterThanOrEqual, Expression.GreaterThanOrEqual, false);
        yield return new BinaryExpressionSerializer(ExpressionType.GreaterThan, Expression.GreaterThan, false);
        yield return new BinaryExpressionSerializer(ExpressionType.LessThanOrEqual, Expression.LessThanOrEqual, false);
        yield return new BinaryExpressionSerializer(ExpressionType.LessThan, Expression.LessThan, false);
        yield return new BinaryExpressionSerializer(ExpressionType.AndAlso, Expression.AndAlso);
        yield return new BinaryExpressionSerializer(ExpressionType.OrElse, Expression.OrElse);
        yield return new BinaryExpressionSerializer(ExpressionType.And, Expression.And);
        yield return new BinaryExpressionSerializer(ExpressionType.Or, Expression.Or);
        yield return new BinaryExpressionSerializer(ExpressionType.ExclusiveOr, Expression.ExclusiveOr);
        yield return new BinaryExpressionSerializer(ExpressionType.Add, Expression.Add);
        yield return new BinaryExpressionSerializer(ExpressionType.AddChecked, Expression.AddChecked);
        yield return new BinaryExpressionSerializer(ExpressionType.Divide, Expression.Divide);
        yield return new BinaryExpressionSerializer(ExpressionType.Modulo, Expression.Modulo);
        yield return new BinaryExpressionSerializer(ExpressionType.Multiply, Expression.Multiply);
        yield return new BinaryExpressionSerializer(ExpressionType.MultiplyChecked, Expression.MultiplyChecked);
        yield return new BinaryExpressionSerializer(ExpressionType.Power, Expression.Power);
        yield return new BinaryExpressionSerializer(ExpressionType.Subtract, Expression.Subtract);
        yield return new BinaryExpressionSerializer(ExpressionType.SubtractChecked, Expression.SubtractChecked);
        yield return new BinaryExpressionSerializer(ExpressionType.LeftShift, Expression.LeftShift);
        yield return new BinaryExpressionSerializer(ExpressionType.RightShift, Expression.RightShift);
        yield return new BinaryExpressionSerializer(ExpressionType.Coalesce, (left, right, _) => Expression.Coalesce(left, right));
        yield return new BinaryExpressionSerializer(ExpressionType.ArrayIndex, (left, right, _) => Expression.ArrayIndex(left, right));
        yield return new BinaryExpressionSerializer(ExpressionType.Assign, (left, right, _) => Expression.Assign(left, right));

        yield return new UnaryExpressionSerializer(ExpressionType.Not, Expression.Not);
        yield return new UnaryExpressionSerializer(ExpressionType.Negate, Expression.Negate);
        yield return new UnaryExpressionSerializer(ExpressionType.NegateChecked, Expression.NegateChecked);
        yield return new UnaryExpressionSerializer(ExpressionType.UnaryPlus, Expression.UnaryPlus);
        yield return new UnaryExpressionSerializer(ExpressionType.Convert, Expression.Convert);
        yield return new UnaryExpressionSerializer(ExpressionType.ConvertChecked, Expression.ConvertChecked);
        yield return new UnaryExpressionSerializer(ExpressionType.TypeAs, (operand, type, _) => Expression.TypeAs(operand, type));

        yield return new InvokeExpressionSerializer();
        yield return new TypeIsExpressionSerializer();
        var newSerializer = new NewExpressionSerializer();
        yield return newSerializer;
        yield return new NewArrayInitExpressionSerializer();
        yield return new MemberInitExpressionSerializer(newSerializer);
        yield return new ListInitExpressionSerializer(newSerializer);
        yield return new ConditionalExpressionSerializer();
        yield return new DefaultExpressionSerializer();
        yield return new BlockExpressionSerializer(parameter);
    }

}
