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
                throw new ArgumentNullException("alias", "Pattern alias is null");
            }
            var fieldMember = alias.Body as MemberExpression;
            if (fieldMember == null)
            {
                throw new ArgumentException(
                    string.Format("Invalid pattern alias expression. Expected={0}, Actual={1}",
                        typeof(MemberExpression), alias.Body.GetType()));
            }
            return Expression.Parameter(fieldMember.Type, fieldMember.Member.Name);
        }
    }
}