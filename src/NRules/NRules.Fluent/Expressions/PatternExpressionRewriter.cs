using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal class PatternExpressionRewriter : ExpressionRewriter
    {
        private readonly Declaration _patternDeclaration;
        private ParameterExpression _originalParameter;
        private ParameterExpression _normalizedParameter;

        public PatternExpressionRewriter(Declaration patternDeclaration, IEnumerable<Declaration> declarations)
            : base(declarations)
        {
            _patternDeclaration = patternDeclaration;
        }

        protected override void InitParameters(LambdaExpression expression)
        {
            _originalParameter = expression.Parameters.Single();
            _normalizedParameter = _patternDeclaration.ToParameterExpression();
            Parameters.Add(_normalizedParameter);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (parameter == _originalParameter)
            {
                return Parameters.First();
            }
            return base.VisitParameter(parameter);
        }
    }
}