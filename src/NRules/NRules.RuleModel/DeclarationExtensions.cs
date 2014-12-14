using System.Linq.Expressions;

namespace NRules.RuleModel
{
    public static class DeclarationExtensions
    {
        /// <summary>
        /// Converts pattern <see cref="Declaration"/> to a <see cref="ParameterExpression"/>.
        /// </summary>
        /// <param name="declaration">Declaration to convert.</param>
        /// <returns>Parameter expression.</returns>
        public static ParameterExpression ToParameterExpression(this Declaration declaration)
        {
            return Expression.Parameter(declaration.Type, declaration.FullName);
        }
    }
}