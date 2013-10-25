using System;
using System.Linq.Expressions;

namespace NRules.Inline.Expressions
{
    internal static class ExpressionUtils
    {
        public static ParameterExpression ExtractSymbol<T>(this Expression<Func<T>> @alias)
        {
            var fieldMember = @alias.Body as MemberExpression;
            if (fieldMember == null)
            {
                throw new InvalidOperationException("Pattern alias must be a variable");
            }
            return Expression.Parameter(fieldMember.Type, fieldMember.Member.Name);
        }
    }
}
