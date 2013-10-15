using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Dsl;
using NRules.Rule;

namespace NRules.Core.Expressions
{
    internal class ActionRewriter : ExpressionVisitor
    {
        private readonly IDictionary<string, Declaration> _declarations;
        private List<ParameterExpression> _parameters;
        private ParameterExpression _context;

        public ActionRewriter(IEnumerable<Declaration> declarations)
        {
            _declarations = declarations.ToDictionary(d => d.Name);
        }

        public LambdaExpression Rewrite(LambdaExpression expression)
        {
            _context = Expression.Parameter(typeof (IActionContext), "context");
            _parameters = new List<ParameterExpression>{_context};
            Expression body = Visit(expression.Body);
            return Expression.Lambda(body, expression.TailCall, _parameters);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Context))
            {
                var method = typeof (IActionContext).GetMethod(m.Method.Name);
                IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
                return Expression.Call(_context, method, args);
            }
            return base.VisitMethodCall(m);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
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

            return base.VisitMemberAccess(m);
        }
    }
}