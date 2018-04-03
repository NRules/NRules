using System;
using System.Linq.Expressions;

namespace NRules.Fluent.Expressions
{
    internal static class ExpressionExtensions
    {
        public static ParameterExpression ToParameterExpression<T>(this Expression<Func<T>> alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException(nameof(alias), "Pattern alias is null");
            }

            if (!(alias.Body is MemberExpression fieldMember))
            {
                throw new ArgumentException(
                    $"Invalid pattern alias expression. Expected={typeof(MemberExpression)}, Actual={alias.Body.GetType()}");
            }
            return Expression.Parameter(fieldMember.Type, fieldMember.Member.Name);
        }
    }
}