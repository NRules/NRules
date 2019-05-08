using System.Linq.Expressions;

namespace NRules.RuleModel
{
    public static class DeclarationExtensions
    {
        /// <summary>
        /// Converts rule element <see cref="Declaration"/> to a <see cref="ParameterExpression"/>.
        /// </summary>
        /// <param name="declaration">Declaration to convert.</param>
        /// <returns>Parameter expression.</returns>
        public static ParameterExpression ToParameterExpression(this Declaration declaration)
        {
            return Expression.Parameter(declaration.Type, declaration.Name);
        }

        /// <summary>
        /// Converts <see cref="ParameterExpression"/> to a rule element <see cref="Declaration"/>.
        /// </summary>
        /// <param name="parameter">Parameter expression to convert</param>
        /// <returns>Rule element declaration.</returns>
        public static Declaration ToDeclaration(this ParameterExpression parameter)
        {
            return new Declaration(parameter.Type, parameter.Name);
        }
    }
}