using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal class ExpressionRewriter : ExpressionVisitor
    {
        private IDictionary<string, Declaration> Declarations { get; }
        protected List<ParameterExpression> Parameters { get; }

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

        protected virtual void InitParameters(LambdaExpression expression)
        {
            Parameters.Clear();
            Parameters.AddRange(expression.Parameters);
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            if (Declarations.TryGetValue(member.Member.Name, out var declaration))
            {
                ParameterExpression parameter = Parameters.FirstOrDefault(p => p.Name == declaration.Name);
                if (parameter == null)
                {
                    parameter = declaration.ToParameterExpression();
                    Parameters.Add(parameter);
                }
                else if (parameter.Type != declaration.Type)
                {
                    throw new ArgumentException(
                        $"Expression parameter type mismatch. Name={declaration.Name}, ExpectedType={declaration.Type}, FoundType={parameter.Type}");
                }
                return parameter;
            }

            return base.VisitMember(member);
        }
    }
}