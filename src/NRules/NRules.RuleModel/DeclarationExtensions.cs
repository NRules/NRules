using System.Linq.Expressions;

namespace NRules.RuleModel
{
    public static class DeclarationExtensions
    {
        /// <summary>
        /// Converts pattern <see cref="Declaration"/> to a <see cref="ParameterExpression"/>.
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        public static ParameterExpression ToParameterExpression(this Declaration declaration)
        {
            return Expression.Parameter(declaration.Type, declaration.Name);
        }
    }
}