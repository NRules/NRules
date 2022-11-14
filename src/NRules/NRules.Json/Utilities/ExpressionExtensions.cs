using System;
using System.Linq.Expressions;

namespace NRules.Json.Utilities;

internal static class ExpressionExtensions
{
    public static Type GetImpliedDelegateType(this LambdaExpression value)
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
