using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal abstract class ExpressionRewriter : ExpressionVisitor
    {
        protected IDictionary<string, Declaration> Declarations { get; private set; }
        protected IList<ParameterExpression> Parameters { get; private set; }

        public ExpressionRewriter(IEnumerable<Declaration> declarations)
        {
            Declarations = declarations.ToDictionary(d => d.Name);
            Parameters = new List<ParameterExpression>();
        }

        public LambdaExpression Rewrite(LambdaExpression expression)
        {
            Parameters.Clear();
            InitParameters(expression);
            Expression body = Visit(expression.Body);
            return Expression.Lambda(body, expression.TailCall, Parameters);
        }

        protected abstract void InitParameters(LambdaExpression expression);
        
        protected override Expression VisitMember(MemberExpression member)
        {
            Declaration declaration;
            if (Declarations.TryGetValue(member.Member.Name, out declaration))
            {
                ParameterExpression parameter = Parameters.FirstOrDefault(p => p.Name == declaration.Name);
                if (parameter == null)
                {
                    parameter = Expression.Parameter(declaration.Type, declaration.Name);
                    Parameters.Add(parameter);
                }
                else if (parameter.Type != declaration.Type)
                {
                    throw new ArgumentException(
                        string.Format("Expression parameter type mismatch. Name={0}, ExpectedType={1}, FoundType={2}",
                            declaration.Name, declaration.Type, parameter.Type));
                }
                return parameter;
            }

            return base.VisitMember(member);
        }
    }
}