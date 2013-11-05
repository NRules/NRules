using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal class ConditionRewriter : ExpressionVisitor
    {
        private readonly IDictionary<string, Declaration> _declarations;
        private List<ParameterExpression> _parameters;
        private ParameterExpression _oldParameter;

        public ConditionRewriter(IEnumerable<Declaration> declarations)
        {
            _declarations = declarations.ToDictionary(d => d.Name);
        }

        public LambdaExpression Rewrite(Declaration declaration, LambdaExpression expression)
        {
            _oldParameter = expression.Parameters.Single();
            _parameters = new List<ParameterExpression> {Expression.Parameter(declaration.Type, declaration.Name)};
            Expression body = Visit(expression.Body);
            return Expression.Lambda(body, expression.TailCall, _parameters);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            Declaration declaration;
            if (_declarations.TryGetValue(m.Member.Name, out declaration))
            {
                ParameterExpression parameter = _parameters.FirstOrDefault(p => p.Name == m.Member.Name);
                if (parameter == null)
                {
                    parameter = Expression.Parameter(declaration.Type, m.Member.Name);
                    _parameters.Add(parameter);
                }
                return parameter;
            }

            return base.VisitMember(m);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p.Name == _oldParameter.Name)
            {
                return _parameters.First();
            }
            return base.VisitParameter(p);
        }
    }
}